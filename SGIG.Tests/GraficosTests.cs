using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.Tests;

/// <summary>
/// Consultas de la sección de gráficos (Fase 11, con rango de fechas desde la Fase 14):
/// cada valor se compara con SQL directo.
/// </summary>
[Collection(ColeccionBaseDeDatos.Nombre)]
public class GraficosTests(BaseDeDatosFixture _)
{
    private readonly ServicioReporte _servicio = new();

    private static DateTime InicioMes => new(DateTime.Today.Year, DateTime.Today.Month, 1);
    private static DateTime FinMes => InicioMes.AddMonths(1).AddDays(-1);

    [Fact]
    public void Ingresos_por_plan_coinciden_con_lo_facturado_en_el_rango()
    {
        var plan = Datos.PlanPor("Mensual");
        var id = new ServicioSocio().Alta(Datos.NuevoSocio());
        new ServicioTesoreria().RegistrarCobro(id, plan.IdPlan, Datos.PrimerMedioPago(), plan.Precio, null);

        var puntos = _servicio.ObtenerIngresosPorPlan(InicioMes, FinMes);

        var enSql = Convert.ToDecimal(Datos.Escalar(
            @"SELECT ISNULL(SUM(monto_total),0) FROM dbo.Facturacion
              WHERE id_plan=@p AND fecha_emision >= @d AND fecha_emision < @h",
            ("@p", plan.IdPlan), ("@d", InicioMes), ("@h", InicioMes.AddMonths(1))));

        Assert.True(enSql >= plan.Precio);
        Assert.Equal(enSql, puntos.Single(p => p.Etiqueta == plan.Nombre).Valor);
    }

    [Fact]
    public void Ingresos_por_plan_del_mes_suman_lo_mismo_que_la_tarjeta_y_vienen_de_mayor_a_menor()
    {
        var puntos = _servicio.ObtenerIngresosPorPlan(InicioMes, FinMes);

        Assert.Equal(_servicio.ObtenerMetricas().IngresosMes, puntos.Sum(p => p.Valor));
        Assert.Equal(puntos.OrderByDescending(p => p.Valor).Select(p => p.Valor), puntos.Select(p => p.Valor));
        // Los planes activos aparecen aunque no hayan facturado nada.
        Assert.All(new ServicioPlan().Listar(), plan => Assert.Contains(puntos, p => p.Etiqueta == plan.Nombre));
    }

    [Fact]
    public void Un_rango_sin_facturacion_deja_todos_los_planes_en_cero()
    {
        var puntos = _servicio.ObtenerIngresosPorPlan(new DateTime(1990, 1, 1), new DateTime(1990, 12, 31));
        Assert.NotEmpty(puntos);
        Assert.All(puntos, p => Assert.Equal(0, p.Valor));
    }

    [Fact]
    public void Accesos_por_hora_cuentan_los_intentos_del_rango_y_no_dejan_huecos()
    {
        var id = new ServicioSocio().Alta(Datos.NuevoSocio());
        var documento = Datos.Escalar("SELECT documento FROM dbo.Persona WHERE id_persona=@i", ("@i", id))!.ToString()!;
        Datos.FijarVencimiento(id, DateTime.Today.AddDays(10));
        new ServicioCheckin().RegistrarIntento(documento);

        var desde = DateTime.Today.AddDays(-6);
        var puntos = _servicio.ObtenerAccesosPorHora(desde, DateTime.Today);

        var horas = puntos.Select(p => int.Parse(p.Etiqueta)).ToList();
        Assert.True(horas.First() <= 6 && horas.Last() >= 22);
        Assert.Equal(Enumerable.Range(horas.First(), horas.Count), horas);

        var horaActual = DateTime.Now.Hour;
        var enSql = Datos.Entero(
            @"SELECT COUNT(*) FROM dbo.Checkin
              WHERE fecha_hora >= @d AND fecha_hora < @h AND DATEPART(HOUR, fecha_hora) = @hora",
            ("@d", desde), ("@h", DateTime.Today.AddDays(1)), ("@hora", horaActual));
        Assert.True(enSql >= 1);
        Assert.Equal(enSql, puntos.Single(p => int.Parse(p.Etiqueta) == horaActual).Valor);
        Assert.Equal(
            Datos.Entero("SELECT COUNT(*) FROM dbo.Checkin WHERE fecha_hora >= @d AND fecha_hora < @h",
                ("@d", desde), ("@h", DateTime.Today.AddDays(1))),
            puntos.Sum(p => p.Valor));
    }

    [Fact]
    public void Un_dia_sin_accesos_devuelve_la_franja_completa_en_cero()
    {
        var dia = new DateTime(1990, 1, 1);
        var puntos = _servicio.ObtenerAccesosPorHora(dia, dia);

        Assert.Equal(17, puntos.Count); // 06 a 22
        Assert.All(puntos, p => Assert.Equal(0, p.Valor));
    }

    [Fact]
    public void Con_un_rango_corto_los_ingresos_van_por_dia_y_coinciden_con_la_base()
    {
        var plan = Datos.PlanPor("Mensual");
        var id = new ServicioSocio().Alta(Datos.NuevoSocio());
        new ServicioTesoreria().RegistrarCobro(id, plan.IdPlan, Datos.PrimerMedioPago(), plan.Precio, null);

        var desde = DateTime.Today.AddDays(-13);
        var puntos = _servicio.ObtenerIngresosEnElTiempo(desde, DateTime.Today);

        Assert.True(ServicioReporte.AgrupaPorDia(desde, DateTime.Today));
        Assert.Equal(14, puntos.Count);
        Assert.Equal(DateTime.Today.ToString("dd/MM"), puntos.Last().Etiqueta);
        Assert.True(puntos.Last().Valor >= plan.Precio);
        Assert.Equal(
            Convert.ToDecimal(Datos.Escalar(
                "SELECT ISNULL(SUM(monto_total),0) FROM dbo.Facturacion WHERE fecha_emision >= @d AND fecha_emision < @h",
                ("@d", desde), ("@h", DateTime.Today.AddDays(1)))),
            puntos.Sum(p => p.Valor));
    }

    [Fact]
    public void Con_un_rango_largo_los_ingresos_van_por_mes_y_el_ultimo_coincide_con_la_tarjeta()
    {
        var plan = Datos.PlanPor("Mensual");
        var id = new ServicioSocio().Alta(Datos.NuevoSocio());
        new ServicioTesoreria().RegistrarCobro(id, plan.IdPlan, Datos.PrimerMedioPago(), plan.Precio, null);

        var desde = InicioMes.AddMonths(-11);
        var puntos = _servicio.ObtenerIngresosEnElTiempo(desde, FinMes);

        Assert.False(ServicioReporte.AgrupaPorDia(desde, FinMes));
        Assert.Equal(12, puntos.Count);
        Assert.Equal(_servicio.ObtenerMetricas().IngresosMes, puntos.Last().Valor);
        Assert.Equal(
            Convert.ToDecimal(Datos.Escalar(
                "SELECT ISNULL(SUM(monto_total),0) FROM dbo.Facturacion WHERE fecha_emision >= @d AND fecha_emision < @h",
                ("@d", desde), ("@h", FinMes.AddDays(1)))),
            puntos.Sum(p => p.Valor));
    }

    [Fact]
    public void Mantenimientos_en_el_tiempo_coinciden_con_la_base()
    {
        var tecnico = new ServicioUsuario().Alta_y_obtener(Roles.Tecnico, "clave1234").IdPersona;
        var maquina = Datos.Entero(@"SELECT TOP 1 id_maquina FROM dbo.Maquina WHERE estado='Operativa' ORDER BY id_maquina DESC");
        var servicioMantenimiento = new ServicioMantenimiento();
        var idMantenimiento = servicioMantenimiento.Registrar(maquina, tecnico, DateTime.Today, "prueba de gráfico");
        // Se libera la máquina para no condicionar a otras pruebas.
        servicioMantenimiento.Finalizar(
            servicioMantenimiento.ObtenerActivos().First(m => m.IdMantenimiento == idMantenimiento), DateTime.Today);

        var desde = InicioMes.AddMonths(-5);
        var puntos = _servicio.ObtenerMantenimientosEnElTiempo(desde, FinMes);

        Assert.Equal(6, puntos.Count);
        var enSql = Datos.Entero(
            "SELECT COUNT(*) FROM dbo.Mantenimiento WHERE fecha_inicio >= @d AND fecha_inicio < @h",
            ("@d", InicioMes), ("@h", InicioMes.AddMonths(1)));
        Assert.True(enSql >= 1);
        Assert.Equal(enSql, puntos.Last().Valor);
        Assert.Equal(
            Datos.Entero("SELECT COUNT(*) FROM dbo.Mantenimiento WHERE fecha_inicio >= @d AND fecha_inicio < @h",
                ("@d", desde), ("@h", InicioMes.AddMonths(1))),
            puntos.Sum(p => p.Valor));
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(31, true)]
    [InlineData(32, false)]
    public void Hasta_31_dias_se_agrupa_por_dia(int dias, bool porDia)
    {
        var desde = new DateTime(2026, 1, 1);
        Assert.Equal(porDia, ServicioReporte.AgrupaPorDia(desde, desde.AddDays(dias - 1)));
    }

    [Fact]
    public void Un_rango_invertido_o_demasiado_largo_es_rechazado()
    {
        Assert.Throws<NegocioException>(() => _servicio.ObtenerIngresosPorPlan(DateTime.Today, DateTime.Today.AddDays(-1)));
        Assert.Throws<NegocioException>(() => _servicio.ObtenerAccesosPorHora(DateTime.Today, DateTime.Today.AddDays(-1)));
        Assert.Throws<NegocioException>(() => _servicio.ObtenerIngresosEnElTiempo(new DateTime(2020, 1, 1), new DateTime(2026, 1, 1)));
        // 24 meses justos sí se aceptan.
        Assert.Equal(24, _servicio.ObtenerMantenimientosEnElTiempo(new DateTime(2024, 1, 1), new DateTime(2025, 12, 31)).Count);
    }
}
