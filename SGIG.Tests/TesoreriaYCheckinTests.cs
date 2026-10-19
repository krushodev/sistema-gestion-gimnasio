using SGIG.Datos;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.Tests;

[Collection(ColeccionBaseDeDatos.Nombre)]
public class TesoreriaTests(BaseDeDatosFixture _)
{
    private readonly ServicioTesoreria _tesoreria = new();
    private readonly ServicioSocio _socios = new();

    private static DateTime Vencimiento(int idPersona) =>
        (DateTime)Datos.Escalar("SELECT fecha_vencimiento_cuota FROM dbo.Socio WHERE id_persona=@i", ("@i", idPersona))!;

    [Fact]
    public void Cobro_mensual_crea_Facturacion_y_Pago_y_vence_en_un_mes()
    {
        var id = _socios.Alta(Datos.NuevoSocio());
        var plan = Datos.PlanPor("Mensual");

        _tesoreria.RegistrarCobro(id, plan.IdPlan, Datos.PrimerMedioPago(), plan.Precio, null);

        Assert.Equal(1, Datos.Entero("SELECT COUNT(*) FROM dbo.Facturacion WHERE id_persona=@i", ("@i", id)));
        Assert.Equal(1, Datos.Entero(
            "SELECT COUNT(*) FROM dbo.Pago p JOIN dbo.Facturacion f ON f.id_facturacion=p.id_facturacion WHERE f.id_persona=@i", ("@i", id)));
        Assert.Equal(DateTime.Today.AddMonths(1), Vencimiento(id).Date);
    }

    [Theory]
    [InlineData("Diario", 0, 0, 1)]
    [InlineData("Semanal", 0, 0, 7)]
    [InlineData("Anual", 1, 0, 0)]
    public void El_vencimiento_se_calcula_por_calendario(string periodicidad, int anios, int meses, int dias)
    {
        var id = _socios.Alta(Datos.NuevoSocio());
        var plan = Datos.PlanPor(periodicidad);

        _tesoreria.RegistrarCobro(id, plan.IdPlan, Datos.PrimerMedioPago(), plan.Precio, null);

        var esperado = DateTime.Today.AddYears(anios).AddMonths(meses).AddDays(dias);
        Assert.Equal(esperado, Vencimiento(id).Date);
    }

    [Fact]
    public void Un_cobro_adelantado_suma_desde_el_vencimiento_vigente_no_desde_hoy()
    {
        var id = _socios.Alta(Datos.NuevoSocio());
        var plan = Datos.PlanPor("Mensual");
        var medio = Datos.PrimerMedioPago();

        _tesoreria.RegistrarCobro(id, plan.IdPlan, medio, plan.Precio, null);
        var primero = Vencimiento(id);
        _tesoreria.RegistrarCobro(id, plan.IdPlan, medio, plan.Precio, primero);

        Assert.Equal(primero.Date.AddMonths(1), Vencimiento(id).Date);
        Assert.Equal(2, _tesoreria.ObtenerHistorialSocio(id).Count());
    }

    [Fact]
    public void El_monto_editable_se_guarda_tal_cual()
    {
        var id = _socios.Alta(Datos.NuevoSocio());
        var plan = Datos.PlanPor("Mensual");

        _tesoreria.RegistrarCobro(id, plan.IdPlan, Datos.PrimerMedioPago(), 1234.50m, null);

        var monto = (decimal)Datos.Escalar(
            "SELECT p.monto FROM dbo.Pago p JOIN dbo.Facturacion f ON f.id_facturacion=p.id_facturacion WHERE f.id_persona=@i", ("@i", id))!;
        Assert.Equal(1234.50m, monto);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Un_monto_no_positivo_es_rechazado(decimal monto)
    {
        var id = _socios.Alta(Datos.NuevoSocio());
        Assert.Throws<NegocioException>(() =>
            _tesoreria.RegistrarCobro(id, Datos.PlanPor("Mensual").IdPlan, Datos.PrimerMedioPago(), monto, null));
    }

    [Fact]
    public void Un_plan_inexistente_es_rechazado()
    {
        var id = _socios.Alta(Datos.NuevoSocio());
        Assert.Throws<NegocioException>(() => _tesoreria.RegistrarCobro(id, 9999, Datos.PrimerMedioPago(), 100, null));
    }

    [Fact]
    public void Cobro_atomico_un_medio_de_pago_invalido_no_deja_Facturacion_huerfana()
    {
        var id = _socios.Alta(Datos.NuevoSocio());
        var antes = Datos.Entero("SELECT COUNT(*) FROM dbo.Facturacion WHERE id_persona=@i", ("@i", id));

        var ex = Assert.ThrowsAny<Exception>(() =>
            _tesoreria.RegistrarCobro(id, Datos.PlanPor("Mensual").IdPlan, 9999, 100, null));

        Assert.True(ex is NegocioException or AccesoDatosException, ex.GetType().Name);
        Assert.Equal(antes, Datos.Entero("SELECT COUNT(*) FROM dbo.Facturacion WHERE id_persona=@i", ("@i", id)));
        Assert.Null(Datos.Escalar("SELECT fecha_vencimiento_cuota FROM dbo.Socio WHERE id_persona=@i", ("@i", id)));
    }
}

[Collection(ColeccionBaseDeDatos.Nombre)]
public class CheckinTests(BaseDeDatosFixture _)
{
    private readonly ServicioCheckin _checkin = new();
    private readonly ServicioSocio _socios = new();

    private static int Checkins(int idPersona) =>
        Datos.Entero("SELECT COUNT(*) FROM dbo.Checkin WHERE id_persona=@i", ("@i", idPersona));

    private (int Id, string Doc) SocioConCuotaAlDia()
    {
        var socio = Datos.NuevoSocio();
        var id = _socios.Alta(socio);
        Datos.FijarVencimiento(id, DateTime.Today.AddDays(10));
        return (id, socio.Documento);
    }

    [Fact]
    public void Socio_con_cuota_al_dia_es_concedido_y_se_registra()
    {
        var (id, doc) = SocioConCuotaAlDia();
        var r = _checkin.RegistrarIntento(doc);

        Assert.True(r.Concedido, r.Mensaje);
        Assert.Equal(10, r.DiasRestantesCuota);
        Assert.Equal(1, Checkins(id));
    }

    [Fact]
    public void La_cuota_que_vence_hoy_todavia_da_acceso()
    {
        var (id, doc) = SocioConCuotaAlDia();
        Datos.FijarVencimiento(id, DateTime.Today);
        Assert.True(_checkin.RegistrarIntento(doc).Concedido);
    }

    [Fact]
    public void Cuota_vencida_es_rechazada_y_el_intento_queda_registrado()
    {
        var (id, doc) = SocioConCuotaAlDia();
        Datos.FijarVencimiento(id, DateTime.Today.AddDays(-3));

        var r = _checkin.RegistrarIntento(doc);

        Assert.False(r.Concedido);
        Assert.Equal(1, Datos.Entero("SELECT COUNT(*) FROM dbo.Checkin WHERE id_persona=@i AND resultado='Rechazado'", ("@i", id)));
    }

    [Fact]
    public void Socio_sin_ninguna_cuota_es_rechazado()
    {
        var socio = Datos.NuevoSocio();
        _socios.Alta(socio);
        Assert.False(_checkin.RegistrarIntento(socio.Documento).Concedido);
    }

    [Fact]
    public void Socio_dado_de_baja_es_rechazado_aunque_tenga_cuota_vigente()
    {
        var (id, doc) = SocioConCuotaAlDia();
        _socios.DarDeBaja(id);
        Assert.False(_checkin.RegistrarIntento(doc).Concedido);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("00000099")]
    public void Documento_vacio_o_inexistente_es_rechazado_sin_registrar(string documento)
    {
        var antes = Datos.Entero("SELECT COUNT(*) FROM dbo.Checkin");
        Assert.Throws<NegocioException>(() => _checkin.RegistrarIntento(documento));
        Assert.Equal(antes, Datos.Entero("SELECT COUNT(*) FROM dbo.Checkin"));
    }

    [Fact]
    public void El_check_in_responde_en_menos_de_2_segundos_RNF01()
    {
        var (_, doc) = SocioConCuotaAlDia();
        _checkin.RegistrarIntento(doc); // calienta el pool de conexiones

        var reloj = System.Diagnostics.Stopwatch.StartNew();
        _checkin.RegistrarIntento(doc);
        reloj.Stop();

        Assert.True(reloj.Elapsed < TimeSpan.FromSeconds(2), $"tardó {reloj.ElapsedMilliseconds} ms");
    }

    [Fact]
    public void Historial_valida_el_rango_y_trae_los_accesos_del_dia()
    {
        var (id, doc) = SocioConCuotaAlDia();
        _checkin.RegistrarIntento(doc);

        Assert.Throws<NegocioException>(() => _checkin.ObtenerHistorial(DateTime.Today, DateTime.Today.AddDays(-1)));
        Assert.Contains(_checkin.ObtenerHistorial(DateTime.Today, DateTime.Today, doc), c => c.IdPersona == id);
    }
}
