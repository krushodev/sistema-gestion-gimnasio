using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.Tests;

/// <summary>Inicio "qué tengo que hacer hoy" (Fase 13): avisos, cuotas por vencer, cumpleaños y últimos cobros.</summary>
[Collection(ColeccionBaseDeDatos.Nombre)]
public class InicioTests(BaseDeDatosFixture _)
{
    private readonly ServicioAvisos _avisos = new();
    private readonly ServicioSocio _socios = new();

    private static int ContarActivos(string condicion, params (string, object?)[] parametros) =>
        Datos.Entero("SELECT COUNT(*) FROM dbo.Socio WHERE activo = 1 AND " + condicion, parametros);

    [Fact]
    public void El_aviso_de_cuotas_vencidas_cuenta_lo_mismo_que_la_base()
    {
        var id = _socios.Alta(Datos.NuevoSocio());
        Datos.FijarVencimiento(id, DateTime.Today.AddDays(-2));

        var aviso = _avisos.ObtenerAvisos(Roles.Administrador, DateTime.Today)
            .Single(a => a.Nivel == "Peligro" && a.Categoria == "Cuotas");

        var enSql = ContarActivos("fecha_vencimiento_cuota < @hoy", ("@hoy", DateTime.Today));
        Assert.StartsWith($"{enSql} ", aviso.Texto);
    }

    [Fact]
    public void Por_vencer_incluye_hoy_y_los_proximos_siete_dias_y_nada_mas()
    {
        var hoy = _socios.Alta(Datos.NuevoSocio());
        var enTres = _socios.Alta(Datos.NuevoSocio());
        var enDiez = _socios.Alta(Datos.NuevoSocio());
        var dadoDeBaja = _socios.Alta(Datos.NuevoSocio());
        Datos.FijarVencimiento(hoy, DateTime.Today);
        Datos.FijarVencimiento(enTres, DateTime.Today.AddDays(3));
        Datos.FijarVencimiento(enDiez, DateTime.Today.AddDays(10));
        Datos.FijarVencimiento(dadoDeBaja, DateTime.Today.AddDays(1));
        _socios.DarDeBaja(dadoDeBaja);

        var porVencer = _socios.ObtenerPorVencer(DateTime.Today, 7);

        Assert.Contains(porVencer, s => s.IdPersona == hoy);
        Assert.Contains(porVencer, s => s.IdPersona == enTres);
        Assert.DoesNotContain(porVencer, s => s.IdPersona == enDiez);
        Assert.DoesNotContain(porVencer, s => s.IdPersona == dadoDeBaja);
        Assert.Equal(porVencer.OrderBy(s => s.FechaVencimientoCuota).Select(s => s.FechaVencimientoCuota),
            porVencer.Select(s => s.FechaVencimientoCuota));
        Assert.Equal(
            ContarActivos("fecha_vencimiento_cuota BETWEEN @d AND @h", ("@d", DateTime.Today), ("@h", DateTime.Today.AddDays(7))),
            porVencer.Count);
    }

    [Fact]
    public void La_recepcion_ve_el_cumpleanios_de_hoy()
    {
        var socio = Datos.NuevoSocio(nombre: "Cumpleañero");
        var id = _socios.Alta(socio);
        Datos.Ejecutar("UPDATE dbo.Persona SET fecha_nacimiento = @f WHERE id_persona = @i",
            ("@f", DateTime.Today.AddYears(-30)), ("@i", id));

        var avisos = _avisos.ObtenerAvisos(Roles.Recepcionista, DateTime.Today);

        Assert.Contains(avisos, a => a.Categoria == "Cumpleanios" && a.Texto.Contains("Cumpleañero Test"));
        // El cumpleaños es un detalle de mostrador: el Administrador no lo recibe.
        Assert.DoesNotContain(_avisos.ObtenerAvisos(Roles.Administrador, DateTime.Today), a => a.Categoria == "Cumpleanios");
    }

    [Theory]
    [InlineData("1990-10-25", "2026-10-25", true)]
    [InlineData("1990-10-25", "2026-10-26", false)]
    [InlineData("2000-02-29", "2027-02-28", true)]  // no bisiesto: lo festeja el 28
    [InlineData("2000-02-29", "2028-02-28", false)] // bisiesto: lo festeja el 29
    [InlineData("2000-02-29", "2028-02-29", true)]
    public void Cumpleanios_contempla_el_29_de_febrero(string nacimiento, string hoy, bool esperado)
    {
        Assert.Equal(esperado, ServicioSocio.EsCumpleanios(DateTime.Parse(nacimiento), DateTime.Parse(hoy)));
    }

    [Fact]
    public void Sin_fecha_de_nacimiento_no_hay_cumpleanios()
    {
        Assert.False(ServicioSocio.EsCumpleanios(null, DateTime.Today));
    }

    [Fact]
    public void El_tecnico_ve_las_reparaciones_demoradas_y_no_los_avisos_de_cuotas()
    {
        var tecnico = new ServicioUsuario().Alta_y_obtener(Roles.Tecnico, "clave1234").IdPersona;
        var maquina = Datos.Entero("SELECT TOP 1 id_maquina FROM dbo.Maquina WHERE estado='Operativa' ORDER BY id_maquina DESC");
        var mantenimientos = new ServicioMantenimiento();
        var id = mantenimientos.Registrar(maquina, tecnico, DateTime.Today.AddDays(-10), "demorada");
        try
        {
            var avisos = _avisos.ObtenerAvisos(Roles.Tecnico, DateTime.Today);

            var enSql = Datos.Entero("SELECT COUNT(*) FROM dbo.Mantenimiento WHERE fecha_fin IS NULL AND fecha_inicio <= @d",
                ("@d", DateTime.Today.AddDays(-7)));
            var aviso = avisos.Single(a => a.Categoria == "Maquinas");
            Assert.StartsWith($"{enSql} ", aviso.Texto);
            Assert.DoesNotContain(avisos, a => a.Categoria == "Cuotas");
        }
        finally
        {
            mantenimientos.Finalizar(mantenimientos.ObtenerActivos().First(m => m.IdMantenimiento == id), DateTime.Today);
        }
    }

    [Fact]
    public void La_recepcion_no_recibe_avisos_de_maquinas()
    {
        Assert.DoesNotContain(_avisos.ObtenerAvisos(Roles.Recepcionista, DateTime.Today), a => a.Categoria == "Maquinas");
    }

    [Fact]
    public void Ultimos_pagos_trae_el_mas_reciente_primero_con_el_nombre_del_socio()
    {
        var plan = Datos.PlanPor("Mensual");
        var id = _socios.Alta(Datos.NuevoSocio(nombre: "Pagador"));
        new ServicioTesoreria().RegistrarCobro(id, plan.IdPlan, Datos.PrimerMedioPago(), plan.Precio, null);

        var pagos = new ServicioTesoreria().ObtenerUltimosPagos(5).ToList();

        Assert.InRange(pagos.Count, 1, 5);
        Assert.Equal("Test, Pagador", pagos[0].NombreSocio);
        Assert.Equal(pagos.OrderByDescending(p => p.FechaPago).Select(p => p.FechaPago), pagos.Select(p => p.FechaPago));
        Assert.Throws<NegocioException>(() => new ServicioTesoreria().ObtenerUltimosPagos(0));
    }
}
