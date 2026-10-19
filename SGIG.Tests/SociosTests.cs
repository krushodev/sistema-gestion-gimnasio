using SGIG.Datos;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.Tests;

[Collection(ColeccionBaseDeDatos.Nombre)]
public class SociosTests(BaseDeDatosFixture _)
{
    private readonly ServicioSocio _servicio = new();

    [Fact]
    public void Alta_valida_crea_Persona_y_Socio()
    {
        var socio = Datos.NuevoSocio();
        var id = _servicio.Alta(socio);

        Assert.True(id > 0);
        Assert.Equal(1, Datos.Entero("SELECT COUNT(*) FROM dbo.Persona WHERE documento=@d", ("@d", socio.Documento)));
        Assert.Equal(1, Datos.Entero("SELECT COUNT(*) FROM dbo.Socio WHERE id_persona=@i", ("@i", id)));
    }

    [Fact]
    public void Documento_duplicado_es_rechazado()
    {
        var socio = Datos.NuevoSocio();
        _servicio.Alta(socio);
        Assert.Throws<CampoDuplicadoException>(() => _servicio.Alta(Datos.NuevoSocio(socio.Documento)));
    }

    [Theory]
    [InlineData("email", "no-es-un-email")]
    [InlineData("telefono", "12")]
    [InlineData("documento", "ABC123XY")]
    [InlineData("nombre", "")]
    public void Datos_invalidos_son_rechazados_con_NegocioException(string campo, string valor)
    {
        var socio = Datos.NuevoSocio();
        switch (campo)
        {
            case "email": socio.Email = valor; break;
            case "telefono": socio.Telefono = valor; break;
            case "documento": socio.Documento = valor; break;
            case "nombre": socio.Nombre = valor; break;
        }
        Assert.Throws<NegocioException>(() => _servicio.Alta(socio));
    }

    [Fact]
    public void Alta_atomica_un_plan_inexistente_no_deja_Persona_huerfana()
    {
        var socio = Datos.NuevoSocio(idPlan: 9999);
        var ex = Assert.ThrowsAny<Exception>(() => _servicio.Alta(socio));
        Assert.True(ex is NegocioException or AccesoDatosException, ex.GetType().Name);
        Assert.Equal(0, Datos.Entero("SELECT COUNT(*) FROM dbo.Persona WHERE documento=@d", ("@d", socio.Documento)));
    }

    [Fact]
    public void Modificar_persiste_los_cambios()
    {
        var id = _servicio.Alta(Datos.NuevoSocio());
        var socio = _servicio.ObtenerPorId(id)!;
        socio.Nombre = "Cambiado";
        _servicio.Modificar(socio);
        Assert.Equal("Cambiado", _servicio.ObtenerPorId(id)!.Nombre);
    }

    [Fact]
    public void Busqueda_por_texto_encuentra_por_documento_y_por_nombre()
    {
        var socio = Datos.NuevoSocio(nombre: "Buscable");
        var id = _servicio.Alta(socio);
        Assert.Contains(_servicio.BuscarPorTexto(socio.Documento), s => s.IdPersona == id);
        Assert.Contains(_servicio.BuscarPorTexto("Buscable"), s => s.IdPersona == id);
    }

    [Fact]
    public void Baja_logica_no_borra_la_fila_y_reactivar_la_restaura()
    {
        var id = _servicio.Alta(Datos.NuevoSocio());

        _servicio.DarDeBaja(id);
        Assert.Equal(1, Datos.Entero("SELECT COUNT(*) FROM dbo.Socio WHERE id_persona=@i AND activo=0", ("@i", id)));
        Assert.DoesNotContain(_servicio.ObtenerActivos(), s => s.IdPersona == id);
        Assert.Contains(_servicio.ObtenerTodos(), s => s.IdPersona == id);

        _servicio.Reactivar(id);
        Assert.True(_servicio.ObtenerPorId(id)!.Activo);
    }

    [Fact]
    public void Reactivar_un_socio_ya_activo_es_rechazado()
    {
        var id = _servicio.Alta(Datos.NuevoSocio());
        Assert.Throws<NegocioException>(() => _servicio.Reactivar(id));
    }

    [Fact]
    public void Una_Persona_existente_se_reutiliza_como_socio_sin_duplicarla()
    {
        // RF#06: alguien que ya es Usuario se da de alta como Socio sin crear otra Persona.
        var usuarios = new ServicioUsuario();
        var usuario = usuarios.Alta_y_obtener(Roles.Recepcionista, "clave1234");

        var persona = _servicio.BuscarPersonaPorDocumento(usuario.Documento);
        Assert.NotNull(persona);

        var socio = Datos.NuevoSocio(usuario.Documento, usuario.Nombre);
        socio.IdPersona = persona!.IdPersona;
        var id = _servicio.Alta(socio);

        Assert.Equal(persona.IdPersona, id);
        Assert.Equal(1, Datos.Entero("SELECT COUNT(*) FROM dbo.Persona WHERE documento=@d", ("@d", usuario.Documento)));
        Assert.Equal(1, Datos.Entero("SELECT COUNT(*) FROM dbo.Socio WHERE id_persona=@i", ("@i", id)));
    }
}
