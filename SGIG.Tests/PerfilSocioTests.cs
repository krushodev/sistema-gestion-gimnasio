using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.Tests;

/// <summary>Perfil del socio (Fase 12): lo que arma <see cref="ServicioSocio.ObtenerPerfil"/>.</summary>
[Collection(ColeccionBaseDeDatos.Nombre)]
public class PerfilSocioTests(BaseDeDatosFixture _)
{
    private readonly ServicioSocio _servicio = new();

    private static string DocumentoDe(int idPersona) =>
        Datos.Escalar("SELECT documento FROM dbo.Persona WHERE id_persona=@i", ("@i", idPersona))!.ToString()!;

    [Fact]
    public void Un_socio_recien_dado_de_alta_no_tiene_cuotas_ni_accesos()
    {
        var plan = Datos.PlanPor("Mensual");
        var id = _servicio.Alta(Datos.NuevoSocio(idPlan: plan.IdPlan));

        var perfil = _servicio.ObtenerPerfil(id, Roles.Administrador);

        Assert.Equal(id, perfil.Socio.IdPersona);
        Assert.Equal("Sin cuotas", perfil.EstadoCuota);
        Assert.Null(perfil.DiasParaVencer);
        Assert.Null(perfil.PlanActual);
        Assert.Equal(plan.Nombre, perfil.PlanPreferido);
        Assert.Empty(perfil.Pagos);
        Assert.Empty(perfil.Accesos);
        Assert.Equal(0, perfil.TotalPagado);
        Assert.False(string.IsNullOrEmpty(perfil.TipoDocumento));
        Assert.NotNull(perfil.Edad); // NuevoSocio carga fecha de nacimiento
    }

    [Fact]
    public void Despues_de_cobrar_y_entrar_muestra_el_plan_el_pago_y_el_acceso()
    {
        var plan = Datos.PlanPor("Mensual");
        var id = _servicio.Alta(Datos.NuevoSocio());
        new ServicioTesoreria().RegistrarCobro(id, plan.IdPlan, Datos.PrimerMedioPago(), plan.Precio, null);
        new ServicioCheckin().RegistrarIntento(DocumentoDe(id));

        var perfil = _servicio.ObtenerPerfil(id, Roles.Administrador);

        Assert.Equal(plan.Nombre, perfil.PlanActual);
        Assert.Equal("Al día", perfil.EstadoCuota);
        Assert.True(perfil.DiasParaVencer > 7);
        Assert.Single(perfil.Pagos);
        Assert.Equal(plan.Precio, perfil.TotalPagado);
        Assert.Single(perfil.Accesos);
        Assert.Equal("Concedido", perfil.Accesos[0].Resultado);
        Assert.Equal(1, perfil.AccesosUltimos30Dias);
    }

    [Fact]
    public void El_recepcionista_recibe_el_perfil_sin_la_parte_economica()
    {
        var plan = Datos.PlanPor("Mensual");
        var id = _servicio.Alta(Datos.NuevoSocio());
        new ServicioTesoreria().RegistrarCobro(id, plan.IdPlan, Datos.PrimerMedioPago(), plan.Precio, null);

        var deRecepcion = _servicio.ObtenerPerfil(id, Roles.Recepcionista);
        var deAdmin = _servicio.ObtenerPerfil(id, Roles.Administrador);

        // Lo operativo lo ve igual que el Administrador...
        Assert.Equal(plan.Nombre, deRecepcion.PlanActual);
        Assert.Equal("Al día", deRecepcion.EstadoCuota);
        // ...pero los pagos y montos no salen de la capa de negocio.
        Assert.False(deRecepcion.IncluyeDatosEconomicos);
        Assert.Empty(deRecepcion.Pagos);
        Assert.Equal(0, deRecepcion.TotalPagado);

        Assert.True(deAdmin.IncluyeDatosEconomicos);
        Assert.Single(deAdmin.Pagos);
    }

    [Fact]
    public void Una_cuota_vencida_se_informa_con_los_dias_en_negativo()
    {
        var id = _servicio.Alta(Datos.NuevoSocio());
        Datos.FijarVencimiento(id, DateTime.Today.AddDays(-3));

        var perfil = _servicio.ObtenerPerfil(id, Roles.Administrador);

        Assert.Equal(-3, perfil.DiasParaVencer);
        Assert.Equal("Vencida", perfil.EstadoCuota);
    }

    [Theory]
    [InlineData(null, "Sin cuotas")]
    [InlineData(-1, "Vencida")]
    [InlineData(0, "Vence hoy")]
    [InlineData(7, "Por vencer")]
    [InlineData(8, "Al día")]
    public void El_estado_de_la_cuota_sigue_el_criterio_del_checkin(int? dias, string esperado)
    {
        Assert.Equal(esperado, ServicioSocio.DescribirCuota(dias));
    }

    [Fact]
    public void Un_socio_inexistente_es_rechazado()
    {
        Assert.Throws<NegocioException>(() => _servicio.ObtenerPerfil(-1, Roles.Administrador));
    }
}
