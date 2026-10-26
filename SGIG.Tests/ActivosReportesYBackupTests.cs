using SGIG.Datos;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.Tests;

[Collection(ColeccionBaseDeDatos.Nombre)]
public class MantenimientoTests(BaseDeDatosFixture _)
{
    private readonly ServicioMantenimiento _servicio = new();

    private static (int IdMaquina, int IdTecnico) Contexto()
    {
        var tecnico = new ServicioUsuario().Alta_y_obtener(Roles.Tecnico, "clave1234").IdPersona;
        // Una máquina operativa distinta por prueba: ninguna depende del estado que dejó otra.
        var maquina = Datos.Entero(@"SELECT TOP 1 m.id_maquina FROM dbo.Maquina m
                                     WHERE m.estado='Operativa'
                                       AND NOT EXISTS (SELECT 1 FROM dbo.Mantenimiento x WHERE x.id_maquina=m.id_maquina)
                                     ORDER BY m.id_maquina");
        return (maquina, tecnico);
    }

    private static string Estado(int idMaquina) =>
        Datos.Escalar("SELECT estado FROM dbo.Maquina WHERE id_maquina=@i", ("@i", idMaquina))!.ToString()!;

    [Fact]
    public void Registrar_pone_la_maquina_en_reparacion_en_la_misma_transaccion()
    {
        var (maquina, tecnico) = Contexto();
        var id = _servicio.Registrar(maquina, tecnico, DateTime.Today, "falla de prueba");

        Assert.True(id > 0);
        Assert.Equal("En Reparacion", Estado(maquina));
        Assert.Contains(_servicio.ObtenerActivos(), m => m.IdMantenimiento == id);
    }

    [Fact]
    public void Finalizar_libera_la_maquina_y_cierra_la_fecha()
    {
        var (maquina, tecnico) = Contexto();
        var id = _servicio.Registrar(maquina, tecnico, DateTime.Today, null);
        var mantenimiento = _servicio.ObtenerActivos().First(m => m.IdMantenimiento == id);

        _servicio.Finalizar(mantenimiento, DateTime.Today);

        Assert.Equal("Operativa", Estado(maquina));
        Assert.NotNull(Datos.Escalar("SELECT fecha_fin FROM dbo.Mantenimiento WHERE id_mantenimiento=@i", ("@i", id)));
        Assert.DoesNotContain(_servicio.ObtenerActivos(), m => m.IdMantenimiento == id);
        Assert.Contains(_servicio.ObtenerPorMaquina(maquina), m => m.IdMantenimiento == id);
    }

    [Fact]
    public void Una_fecha_de_fin_anterior_al_inicio_es_rechazada()
    {
        var (maquina, tecnico) = Contexto();
        var id = _servicio.Registrar(maquina, tecnico, DateTime.Today, null);
        var mantenimiento = _servicio.ObtenerActivos().First(m => m.IdMantenimiento == id);

        Assert.Throws<NegocioException>(() => _servicio.Finalizar(mantenimiento, DateTime.Today.AddDays(-3)));
        Assert.Equal("En Reparacion", Estado(maquina));
    }

    [Fact]
    public void Registrar_sin_elegir_maquina_es_rechazado()
    {
        var (_, tecnico) = Contexto();
        Assert.Throws<NegocioException>(() => _servicio.Registrar(0, tecnico, DateTime.Today, null));
    }
}

[Collection(ColeccionBaseDeDatos.Nombre)]
public class ReportesYCatalogosTests(BaseDeDatosFixture _)
{
    [Fact]
    public void El_total_del_reporte_coincide_con_la_suma_real_de_pagos()
    {
        // Genera al menos un pago propio para que el período no esté vacío.
        var id = new ServicioSocio().Alta(Datos.NuevoSocio());
        var plan = Datos.PlanPor("Mensual");
        new ServicioTesoreria().RegistrarCobro(id, plan.IdPlan, Datos.PrimerMedioPago(), plan.Precio, null);

        var desde = DateTime.Today.AddDays(-60);
        var hasta = DateTime.Today;
        var delReporte = new ServicioReporte().GenerarReporte(desde, hasta).Sum(f => f.MontoTotal);
        var enSql = Convert.ToDecimal(Datos.Escalar(
            "SELECT ISNULL(SUM(monto),0) FROM dbo.Pago WHERE CAST(fecha_pago AS date) BETWEEN @d AND @h",
            ("@d", desde), ("@h", hasta)));

        Assert.True(enSql > 0);
        Assert.Equal(enSql, delReporte);
    }

    [Fact]
    public void Las_metricas_coinciden_con_los_conteos_de_la_base()
    {
        var m = new ServicioReporte().ObtenerMetricas();
        Assert.Equal(Datos.Entero("SELECT COUNT(*) FROM dbo.Socio WHERE activo=1"), m.SociosActivos);
        Assert.InRange(m.SociosAlDia, 0, m.SociosActivos);
        Assert.True(m.IngresosMes >= 0);
    }

    [Fact]
    public void Los_catalogos_sembrados_tienen_datos()
    {
        var catalogo = new ServicioCatalogo();
        Assert.NotEmpty(catalogo.ObtenerProvincias());
        Assert.NotEmpty(catalogo.ObtenerLocalidades());
        Assert.NotEmpty(catalogo.ObtenerTiposDocumento());
        Assert.NotEmpty(catalogo.ObtenerMediosPago());
    }

    [Fact]
    public void Los_planes_sembrados_tienen_periodicidad_valida()
    {
        var planes = new ServicioPlan().Listar().ToList();
        Assert.Equal(4, planes.Count);
        Assert.All(planes, p => Assert.Contains(p.TipoPeriodicidad, new[] { "Diario", "Semanal", "Mensual", "Anual" }));
    }
}

/// <summary>
/// Backup y restore sobre la base de pruebas. El restore deja la base como estaba al hacer el
/// backup, así que esta prueba crea y verifica todo adentro (no deja nada de lo que otras dependan).
/// </summary>
[Collection(ColeccionBaseDeDatos.Nombre)]
public class BackupTests(BaseDeDatosFixture _)
{
    private readonly ServicioBackup _servicio = new();

    [Fact]
    public void Generar_copia_crea_el_archivo_y_restaurar_deshace_los_cambios_posteriores()
    {
        var ruta = Path.Combine(Path.GetTempPath(), $"sgig_test_{Guid.NewGuid():N}.bak");
        try
        {
            _servicio.GenerarCopiaSeguridad(ruta);
            Assert.True(File.Exists(ruta) && new FileInfo(ruta).Length > 0);

            var antes = Datos.Entero("SELECT COUNT(*) FROM dbo.Persona");
            new ServicioSocio().Alta(Datos.NuevoSocio());
            Assert.Equal(antes + 1, Datos.Entero("SELECT COUNT(*) FROM dbo.Persona"));

            Microsoft.Data.SqlClient.SqlConnection.ClearAllPools();
            _servicio.RestaurarCopiaSeguridad(ruta);
            Microsoft.Data.SqlClient.SqlConnection.ClearAllPools();

            Assert.Equal(antes, Datos.Entero("SELECT COUNT(*) FROM dbo.Persona"));
        }
        finally
        {
            if (File.Exists(ruta)) File.Delete(ruta);
        }
    }

    /// <summary>
    /// Igual que en la app: después de restaurar se sigue usando el sistema sin limpiar nada
    /// a mano (las conexiones del pool apuntaban a la base de antes del restore).
    /// </summary>
    [Fact]
    public void Despues_de_restaurar_la_app_sigue_funcionando_sin_intervencion()
    {
        var ruta = Path.Combine(Path.GetTempPath(), $"sgig_test_{Guid.NewGuid():N}.bak");
        try
        {
            _servicio.GenerarCopiaSeguridad(ruta);
            // Deja conexiones abiertas en el pool, como la app en uso.
            Assert.NotEmpty(new ServicioSocio().ObtenerTodos());

            _servicio.RestaurarCopiaSeguridad(ruta);

            // Primera operación después del restore, sin ClearAllPools manual.
            Assert.NotEmpty(new ServicioSocio().ObtenerTodos());
            Assert.NotEmpty(new ServicioPlan().Listar());
        }
        finally
        {
            if (File.Exists(ruta)) File.Delete(ruta);
        }
    }

    private static string AccesoDeLaBase() =>
        Datos.Escalar("SELECT user_access_desc FROM sys.databases WHERE name = DB_NAME()")!.ToString()!;

    /// <summary>
    /// Un archivo que no es un backup se rechaza leyendo su encabezado, antes de tocar la
    /// base: no queda en SINGLE_USER y sigue disponible para el resto del sistema.
    /// </summary>
    [Fact]
    public void Un_archivo_que_no_es_backup_se_rechaza_sin_tocar_la_base()
    {
        var ruta = Path.Combine(Path.GetTempPath(), $"sgig_test_{Guid.NewGuid():N}.bak");
        File.WriteAllText(ruta, "esto no es un backup de SQL Server");
        try
        {
            var ex = Assert.Throws<NegocioException>(() => _servicio.RestaurarCopiaSeguridad(ruta));
            Assert.Contains("no es una copia de seguridad válida", ex.Message);

            Assert.Equal("MULTI_USER", AccesoDeLaBase());
            Assert.NotEmpty(new ServicioPlan().Listar());
        }
        finally
        {
            if (File.Exists(ruta)) File.Delete(ruta);
        }
    }

    /// <summary>
    /// El backup de OTRA base (acá, la base de sistema "model") no puede pisar los datos:
    /// se rechaza por el nombre que figura en su encabezado.
    /// </summary>
    [Fact]
    public void El_backup_de_otra_base_se_rechaza_y_no_se_pisan_los_datos()
    {
        var ruta = Path.Combine(Path.GetTempPath(), $"sgig_test_model_{Guid.NewGuid():N}.bak");
        try
        {
            Datos.Ejecutar("BACKUP DATABASE [model] TO DISK = @r WITH INIT", ("@r", ruta));
            var personasAntes = Datos.Entero("SELECT COUNT(*) FROM dbo.Persona");

            var ex = Assert.Throws<NegocioException>(() => _servicio.RestaurarCopiaSeguridad(ruta));

            Assert.Contains("\"model\"", ex.Message);
            Assert.Equal(personasAntes, Datos.Entero("SELECT COUNT(*) FROM dbo.Persona"));
            Assert.Equal("MULTI_USER", AccesoDeLaBase());
        }
        finally
        {
            if (File.Exists(ruta)) File.Delete(ruta);
        }
    }

    /// <summary>
    /// Sin permiso de escritura en la carpeta, el usuario recibe un mensaje en castellano que
    /// le dice qué hacer, no el error técnico de SQL Server.
    /// </summary>
    [Fact]
    public void Sin_permiso_en_la_carpeta_el_mensaje_es_claro()
    {
        var ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), $"sgig_test_{Guid.NewGuid():N}.bak");
        try
        {
            _servicio.GenerarCopiaSeguridad(ruta);
            // Si SQL Server corre con permisos de administrador, la carpeta sí es escribible:
            // no hay error que verificar en esta máquina.
        }
        catch (AccesoDatosException ex)
        {
            Assert.StartsWith("SQL Server no tiene permiso para escribir en esa carpeta", ex.Message);
        }
        finally
        {
            if (File.Exists(ruta)) File.Delete(ruta);
        }
    }

    [Fact]
    public void Rutas_invalidas_son_rechazadas_con_NegocioException()
    {
        Assert.Throws<NegocioException>(() => _servicio.GenerarCopiaSeguridad(""));
        Assert.Throws<NegocioException>(() => _servicio.GenerarCopiaSeguridad(@"C:\directorio_que_no_existe_sgig\x.bak"));
        Assert.Throws<NegocioException>(() => _servicio.RestaurarCopiaSeguridad(@"C:\no_existe_sgig.bak"));
    }
}
