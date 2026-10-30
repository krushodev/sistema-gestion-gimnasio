using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.Tests;

/// <summary>Los 5 reportes por período de la Fase 14: filas e indicadores contra SQL directo.</summary>
[Collection(ColeccionBaseDeDatos.Nombre)]
public class ReportesPeriodoTests(BaseDeDatosFixture _)
{
    private readonly ServicioReporte _servicio = new();
    private static readonly DateTime Desde = DateTime.Today.AddDays(-30);
    private static readonly DateTime Hasta = DateTime.Today;

    private static (int IdSocio, string Documento) SocioConCobro(decimal? monto = null)
    {
        var plan = Datos.PlanPor("Mensual");
        var id = new ServicioSocio().Alta(Datos.NuevoSocio());
        new ServicioTesoreria().RegistrarCobro(id, plan.IdPlan, Datos.PrimerMedioPago(), monto ?? plan.Precio, null);
        var documento = Datos.Escalar("SELECT documento FROM dbo.Persona WHERE id_persona=@i", ("@i", id))!.ToString()!;
        return (id, documento);
    }

    private static decimal PagosEnSql() => Convert.ToDecimal(Datos.Escalar(
        "SELECT ISNULL(SUM(monto),0) FROM dbo.Pago WHERE fecha_pago >= @d AND fecha_pago < @h",
        ("@d", Desde), ("@h", Hasta.AddDays(1))));

    [Fact]
    public void Ingresos_por_plan_tiene_porcentajes_que_suman_cien()
    {
        SocioConCobro();

        var reporte = _servicio.ReporteIngresosPorPlan(Desde, Hasta);

        Assert.NotEmpty(reporte.Filas);
        Assert.InRange(reporte.Filas.Sum(f => f.Porcentaje), 99.5m, 100.5m); // redondeo a un decimal por fila
        Assert.Equal(3, reporte.Indicadores.Count);
        Assert.Equal("Total facturado", reporte.Indicadores[0].Titulo);
    }

    [Fact]
    public void Ingresos_por_medio_de_pago_suma_lo_cobrado_en_el_periodo()
    {
        SocioConCobro();

        var reporte = _servicio.ReporteIngresosPorMedioPago(Desde, Hasta);

        Assert.Equal(PagosEnSql(), reporte.Filas.Sum(f => f.Monto));
        Assert.Equal(
            Datos.Entero("SELECT COUNT(*) FROM dbo.Pago WHERE fecha_pago >= @d AND fecha_pago < @h", ("@d", Desde), ("@h", Hasta.AddDays(1))),
            reporte.Filas.Sum(f => f.Cantidad));
        Assert.Equal(reporte.Filas.OrderByDescending(f => f.Monto).Select(f => f.Monto), reporte.Filas.Select(f => f.Monto));
    }

    [Fact]
    public void Detalle_de_cobros_trae_cada_pago_con_el_socio()
    {
        var (_, documento) = SocioConCobro(1234.50m);

        var reporte = _servicio.ReporteDetalleCobros(Desde, Hasta);

        var fila = reporte.Filas.Single(f => f.Documento == documento);
        Assert.Equal(1234.50m, fila.Monto);
        Assert.Equal("Test, Prueba", fila.Socio);
        Assert.Equal(PagosEnSql(), reporte.Filas.Sum(f => f.Monto));
        Assert.Equal(reporte.Filas.Select(f => f.Documento).Distinct().Count().ToString(),
            reporte.Indicadores.Single(i => i.Titulo == "Socios que pagaron").Valor);
    }

    [Fact]
    public void Asistencia_separa_concedidos_y_rechazados_por_dia()
    {
        var alDia = new ServicioSocio().Alta(Datos.NuevoSocio());
        var vencido = new ServicioSocio().Alta(Datos.NuevoSocio());
        Datos.FijarVencimiento(alDia, DateTime.Today.AddDays(10));
        Datos.FijarVencimiento(vencido, DateTime.Today.AddDays(-10));
        string Doc(int id) => Datos.Escalar("SELECT documento FROM dbo.Persona WHERE id_persona=@i", ("@i", id))!.ToString()!;
        new ServicioCheckin().RegistrarIntento(Doc(alDia));
        new ServicioCheckin().RegistrarIntento(Doc(vencido));

        var reporte = _servicio.ReporteAsistencia(Desde, Hasta);

        var hoy = reporte.Filas.Single(f => f.Fecha == DateTime.Today);
        Assert.Equal(hoy.Concedidos + hoy.Rechazados, hoy.Total);
        Assert.True(hoy.Concedidos >= 1 && hoy.Rechazados >= 1);
        Assert.Equal(ServicioReporte.Porcentaje(hoy.Rechazados, hoy.Total), hoy.PorcentajeRechazo);
        Assert.Equal(
            Datos.Entero("SELECT COUNT(*) FROM dbo.Checkin WHERE fecha_hora >= @d AND fecha_hora < @h", ("@d", Desde), ("@h", Hasta.AddDays(1))),
            reporte.Filas.Sum(f => f.Total));
    }

    [Fact]
    public void Mantenimientos_calcula_dias_y_estado()
    {
        var tecnico = new ServicioUsuario().Alta_y_obtener(Roles.Tecnico, "clave1234").IdPersona;
        var maquina = Datos.Entero("SELECT TOP 1 id_maquina FROM dbo.Maquina WHERE estado='Operativa' ORDER BY id_maquina DESC");
        var mantenimientos = new ServicioMantenimiento();
        var id = mantenimientos.Registrar(maquina, tecnico, DateTime.Today.AddDays(-5), "reporte de prueba");
        try
        {
            var enCurso = _servicio.ReporteMantenimientos(Desde, Hasta, DateTime.Today)
                .Filas.Single(f => f.Detalle == "reporte de prueba");
            Assert.Equal("En curso", enCurso.Estado);
            Assert.Equal(5, enCurso.Dias);
        }
        finally
        {
            mantenimientos.Finalizar(mantenimientos.ObtenerActivos().First(m => m.IdMantenimiento == id), DateTime.Today.AddDays(-2));
        }

        var finalizado = _servicio.ReporteMantenimientos(Desde, Hasta, DateTime.Today)
            .Filas.Single(f => f.Detalle == "reporte de prueba");
        Assert.Equal("Finalizado", finalizado.Estado);
        Assert.Equal(3, finalizado.Dias);
    }

    [Fact]
    public void Un_periodo_sin_datos_devuelve_indicadores_sin_romperse()
    {
        var desde = new DateTime(1990, 1, 1);
        var hasta = new DateTime(1990, 1, 31);

        Assert.Empty(_servicio.ReporteIngresosPorPlan(desde, hasta).Filas);
        Assert.Equal("—", _servicio.ReporteIngresosPorMedioPago(desde, hasta).Indicadores[1].Valor);
        Assert.Equal("$ 0", _servicio.ReporteDetalleCobros(desde, hasta).Indicadores[2].Valor);
        Assert.Equal("0", _servicio.ReporteAsistencia(desde, hasta).Indicadores[0].Valor);
        Assert.Equal("—", _servicio.ReporteMantenimientos(desde, hasta, DateTime.Today).Indicadores[1].Valor);
    }

    [Fact]
    public void Todos_los_reportes_rechazan_un_rango_invertido()
    {
        var desde = DateTime.Today;
        var hasta = DateTime.Today.AddDays(-1);
        Assert.Throws<NegocioException>(() => _servicio.ReporteIngresosPorPlan(desde, hasta));
        Assert.Throws<NegocioException>(() => _servicio.ReporteIngresosPorMedioPago(desde, hasta));
        Assert.Throws<NegocioException>(() => _servicio.ReporteDetalleCobros(desde, hasta));
        Assert.Throws<NegocioException>(() => _servicio.ReporteAsistencia(desde, hasta));
        Assert.Throws<NegocioException>(() => _servicio.ReporteMantenimientos(desde, hasta, DateTime.Today));
    }
}
