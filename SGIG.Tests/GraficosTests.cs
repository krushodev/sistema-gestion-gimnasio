using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.Tests;

/// <summary>Consultas de los gráficos del Inicio (Fase 11): cada valor se compara con SQL directo.</summary>
[Collection(ColeccionBaseDeDatos.Nombre)]
public class GraficosTests(BaseDeDatosFixture _)
{
    private readonly ServicioReporte _servicio = new();

    [Fact]
    public void Ingresos_por_plan_coinciden_con_lo_facturado_en_el_mes()
    {
        var plan = Datos.PlanPor("Mensual");
        var id = new ServicioSocio().Alta(Datos.NuevoSocio());
        new ServicioTesoreria().RegistrarCobro(id, plan.IdPlan, Datos.PrimerMedioPago(), plan.Precio, null);

        var puntos = _servicio.ObtenerIngresosPorPlan(DateTime.Today);

        var desde = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var enSql = Convert.ToDecimal(Datos.Escalar(
            @"SELECT ISNULL(SUM(monto_total),0) FROM dbo.Facturacion
              WHERE id_plan=@p AND fecha_emision >= @d AND fecha_emision < @h",
            ("@p", plan.IdPlan), ("@d", desde), ("@h", desde.AddMonths(1))));

        Assert.True(enSql >= plan.Precio);
        Assert.Equal(enSql, puntos.Single(p => p.Etiqueta == plan.Nombre).Valor);
    }

    [Fact]
    public void Ingresos_por_plan_suman_lo_mismo_que_la_tarjeta_y_vienen_de_mayor_a_menor()
    {
        var puntos = _servicio.ObtenerIngresosPorPlan(DateTime.Today);

        Assert.Equal(_servicio.ObtenerMetricas().IngresosMes, puntos.Sum(p => p.Valor));
        Assert.Equal(puntos.OrderByDescending(p => p.Valor).Select(p => p.Valor), puntos.Select(p => p.Valor));
        // Los planes activos aparecen aunque no hayan facturado nada.
        Assert.All(new ServicioPlan().Listar(), plan => Assert.Contains(puntos, p => p.Etiqueta == plan.Nombre));
    }

    [Fact]
    public void Accesos_por_hora_cuentan_los_intentos_de_hoy_y_no_dejan_huecos()
    {
        var id = new ServicioSocio().Alta(Datos.NuevoSocio());
        var documento = Datos.Escalar("SELECT documento FROM dbo.Persona WHERE id_persona=@i", ("@i", id))!.ToString()!;
        Datos.FijarVencimiento(id, DateTime.Today.AddDays(10));
        new ServicioCheckin().RegistrarIntento(documento);

        var puntos = _servicio.ObtenerAccesosPorHora(DateTime.Today);

        var horas = puntos.Select(p => int.Parse(p.Etiqueta)).ToList();
        Assert.True(horas.First() <= 6 && horas.Last() >= 22);
        Assert.Equal(Enumerable.Range(horas.First(), horas.Count), horas);

        var horaActual = DateTime.Now.Hour;
        var enSql = Datos.Entero(
            @"SELECT COUNT(*) FROM dbo.Checkin
              WHERE fecha_hora >= @d AND fecha_hora < @h AND DATEPART(HOUR, fecha_hora) = @hora",
            ("@d", DateTime.Today), ("@h", DateTime.Today.AddDays(1)), ("@hora", horaActual));
        Assert.True(enSql >= 1);
        Assert.Equal(enSql, puntos.Single(p => int.Parse(p.Etiqueta) == horaActual).Valor);
        Assert.Equal(
            Datos.Entero("SELECT COUNT(*) FROM dbo.Checkin WHERE fecha_hora >= @d AND fecha_hora < @h",
                ("@d", DateTime.Today), ("@h", DateTime.Today.AddDays(1))),
            puntos.Sum(p => p.Valor));
    }

    [Fact]
    public void Un_dia_sin_accesos_devuelve_la_franja_completa_en_cero()
    {
        var puntos = _servicio.ObtenerAccesosPorHora(new DateTime(1990, 1, 1));

        Assert.Equal(17, puntos.Count); // 06 a 22
        Assert.All(puntos, p => Assert.Equal(0, p.Valor));
    }

    [Fact]
    public void Mantenimientos_por_mes_devuelve_seis_meses_y_coincide_con_la_base()
    {
        var tecnico = new ServicioUsuario().Alta_y_obtener(Roles.Tecnico, "clave1234").IdPersona;
        var maquina = Datos.Entero(@"SELECT TOP 1 id_maquina FROM dbo.Maquina WHERE estado='Operativa' ORDER BY id_maquina DESC");
        var servicioMantenimiento = new ServicioMantenimiento();
        var idMantenimiento = servicioMantenimiento.Registrar(maquina, tecnico, DateTime.Today, "prueba de gráfico");
        // Se libera la máquina para no condicionar a otras pruebas.
        servicioMantenimiento.Finalizar(
            servicioMantenimiento.ObtenerActivos().First(m => m.IdMantenimiento == idMantenimiento), DateTime.Today);

        var puntos = _servicio.ObtenerMantenimientosPorMes(DateTime.Today);

        Assert.Equal(6, puntos.Count);
        var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var enSql = Datos.Entero(
            "SELECT COUNT(*) FROM dbo.Mantenimiento WHERE fecha_inicio >= @d AND fecha_inicio < @h",
            ("@d", inicioMes), ("@h", inicioMes.AddMonths(1)));
        Assert.True(enSql >= 1);
        Assert.Equal(enSql, puntos.Last().Valor);
        Assert.Equal(
            Datos.Entero("SELECT COUNT(*) FROM dbo.Mantenimiento WHERE fecha_inicio >= @d AND fecha_inicio < @h",
                ("@d", inicioMes.AddMonths(-5)), ("@h", inicioMes.AddMonths(1))),
            puntos.Sum(p => p.Valor));
    }

    [Fact]
    public void Mantenimientos_por_mes_rechaza_una_cantidad_de_meses_invalida()
    {
        Assert.Throws<NegocioException>(() => _servicio.ObtenerMantenimientosPorMes(DateTime.Today, 0));
        Assert.Throws<NegocioException>(() => _servicio.ObtenerMantenimientosPorMes(DateTime.Today, 25));
    }
}
