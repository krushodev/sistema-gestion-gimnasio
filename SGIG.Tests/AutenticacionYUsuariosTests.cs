using System.Text;
using SGIG.Datos;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.Tests;

[Collection(ColeccionBaseDeDatos.Nombre)]
public class AutenticacionTests(BaseDeDatosFixture _)
{
    private readonly ServicioAutenticacion _auth = new();

    [Fact]
    public void Admin_del_seed_inicia_sesion_y_trae_su_rol()
    {
        var u = _auth.Autenticar("admin", "admin1234");
        Assert.NotNull(u);
        Assert.Equal(Roles.Administrador, u!.IdRol);
    }

    [Theory]
    [InlineData("admin", "incorrecta")]
    [InlineData("usuario_que_no_existe", "x")]
    [InlineData("", "")]
    [InlineData("admin' OR '1'='1", "x")]       // inyección SQL en el usuario
    [InlineData("admin", "' OR '1'='1")]        // inyección SQL en la contraseña
    public void Credenciales_invalidas_no_autentican(string usuario, string contrasenia) =>
        Assert.Null(_auth.Autenticar(usuario, contrasenia));

    [Fact]
    public void Usuario_dado_de_baja_no_puede_entrar_y_reactivado_si()
    {
        var servicio = new ServicioUsuario();
        var u = servicio.Alta_y_obtener(Roles.Recepcionista, "clave1234");

        Assert.NotNull(_auth.Autenticar(u.NombreUsuario, "clave1234"));
        servicio.DarDeBaja(u.IdPersona, idPersonaLogueada: 1);
        Assert.Null(_auth.Autenticar(u.NombreUsuario, "clave1234"));
        servicio.Reactivar(u.IdPersona);
        Assert.NotNull(_auth.Autenticar(u.NombreUsuario, "clave1234"));
    }
}

[Collection(ColeccionBaseDeDatos.Nombre)]
public class UsuarioTests(BaseDeDatosFixture _)
{
    private readonly ServicioUsuario _servicio = new();
    private readonly ServicioAutenticacion _auth = new();

    [Fact]
    public void Alta_con_rol_Recepcionista_permite_login_inmediato()
    {
        var u = _servicio.Alta_y_obtener(Roles.Recepcionista, "clave1234");
        var sesion = _auth.Autenticar(u.NombreUsuario, "clave1234");
        Assert.NotNull(sesion);
        Assert.Equal(Roles.Recepcionista, sesion!.IdRol);
    }

    [Fact]
    public void La_contrasenia_se_guarda_hasheada_nunca_en_texto_plano()
    {
        var u = _servicio.Alta_y_obtener(Roles.Tecnico, "secretoXYZ");
        var hash = (byte[])Datos.Escalar("SELECT contrasenia_hash FROM dbo.Usuario WHERE id_persona=@i", ("@i", u.IdPersona))!;
        Assert.Equal(32, hash.Length);
        Assert.DoesNotContain("secretoXYZ", Encoding.UTF8.GetString(hash));
    }

    [Fact]
    public void Nombre_de_usuario_duplicado_es_rechazado()
    {
        var existente = _servicio.Alta_y_obtener(Roles.Recepcionista, "clave1234");
        var otro = Datos.NuevoUsuario(Roles.Tecnico, nombreUsuario: existente.NombreUsuario);
        Assert.ThrowsAny<NegocioException>(() => _servicio.Alta(otro, "clave1234"));
    }

    [Fact]
    public void Cambiar_contrasenia_exige_la_actual_correcta()
    {
        var u = _servicio.Alta_y_obtener(Roles.Recepcionista, "clave1234");
        Assert.Throws<NegocioException>(() => _servicio.CambiarContrasenia(u.IdPersona, "incorrecta", "nueva1234"));

        _servicio.CambiarContrasenia(u.IdPersona, "clave1234", "nueva1234");
        Assert.NotNull(_auth.Autenticar(u.NombreUsuario, "nueva1234"));
        Assert.Null(_auth.Autenticar(u.NombreUsuario, "clave1234"));
    }

    [Fact]
    public void Contrasenia_nueva_demasiado_corta_es_rechazada()
    {
        var u = _servicio.Alta_y_obtener(Roles.Recepcionista, "clave1234");
        Assert.Throws<NegocioException>(() => _servicio.CambiarContrasenia(u.IdPersona, "clave1234", "ab"));
    }

    [Fact]
    public void No_puede_darse_de_baja_a_si_mismo()
    {
        var u = _servicio.Alta_y_obtener(Roles.Administrador, "clave1234");
        Assert.Throws<NegocioException>(() => _servicio.DarDeBaja(u.IdPersona, u.IdPersona));
    }

    [Fact]
    public void Baja_logica_conserva_la_fila()
    {
        var u = _servicio.Alta_y_obtener(Roles.Recepcionista, "clave1234");
        _servicio.DarDeBaja(u.IdPersona, idPersonaLogueada: 1);
        Assert.Equal(1, Datos.Entero("SELECT COUNT(*) FROM dbo.Usuario WHERE id_persona=@i AND activo=0", ("@i", u.IdPersona)));
    }

    [Fact]
    public void Reactivar_un_usuario_ya_activo_es_rechazado()
    {
        var u = _servicio.Alta_y_obtener(Roles.Recepcionista, "clave1234");
        Assert.Throws<NegocioException>(() => _servicio.Reactivar(u.IdPersona));
    }

    [Fact]
    public void Alta_atomica_un_rol_inexistente_no_deja_Persona_huerfana()
    {
        var u = Datos.NuevoUsuario(idRol: 9999);
        var ex = Assert.ThrowsAny<Exception>(() => _servicio.Alta(u, "clave1234"));
        Assert.True(ex is NegocioException or AccesoDatosException, ex.GetType().Name);
        Assert.Equal(0, Datos.Entero("SELECT COUNT(*) FROM dbo.Persona WHERE documento=@d", ("@d", u.Documento)));
    }
}

internal static class ServicioUsuarioExtensiones
{
    /// <summary>Da de alta un usuario nuevo y lo devuelve ya con su IdPersona.</summary>
    public static Usuario Alta_y_obtener(this ServicioUsuario servicio, int idRol, string contrasenia)
    {
        var usuario = Datos.NuevoUsuario(idRol);
        usuario.IdPersona = 0;
        usuario.IdPersona = servicio.Alta(usuario, contrasenia);
        return usuario;
    }
}
