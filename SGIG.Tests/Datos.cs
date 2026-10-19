using Microsoft.Data.SqlClient;
using SGIG.Datos;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.Tests;

/// <summary>Ayudas compartidas por las pruebas de integración (datos únicos y consultas directas).</summary>
internal static class Datos
{
    private static int _contador = 20_000_000;

    /// <summary>DNI numérico único de 8 dígitos (válido para <see cref="Validaciones"/>).</summary>
    public static string Doc() => Interlocked.Increment(ref _contador).ToString();

    public static Socio NuevoSocio(string? documento = null, string nombre = "Prueba", int? idPlan = null)
    {
        documento ??= Doc();
        return new Socio
        {
            Documento = documento,
            IdTipoDocumento = Validaciones.IdTipoDocumentoDni,
            Nombre = nombre,
            Apellido = "Test",
            Email = $"t{documento}@test.com",
            Telefono = "3794123456",
            FechaNacimiento = new DateTime(1995, 5, 5),
            IdPlan = idPlan,
            Activo = true
        };
    }

    public static Usuario NuevoUsuario(int idRol, string? nombreUsuario = null, string? documento = null)
    {
        documento ??= Doc();
        return new Usuario
        {
            Documento = documento,
            IdTipoDocumento = Validaciones.IdTipoDocumentoDni,
            Nombre = "Usuario",
            Apellido = "Test",
            Email = $"u{documento}@test.com",
            Telefono = "3794111222",
            NombreUsuario = nombreUsuario ?? "u" + documento,
            IdRol = idRol,
            Legajo = "LEG-" + documento,
            FechaIngreso = DateTime.Today
        };
    }

    public static object? Escalar(string sql, params (string Nombre, object? Valor)[] parametros)
    {
        using var conexion = Conexion.ObtenerConexionAbierta();
        using var comando = new SqlCommand(sql, conexion);
        foreach (var (nombre, valor) in parametros) comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        var resultado = comando.ExecuteScalar();
        return resultado is DBNull ? null : resultado;
    }

    public static int Entero(string sql, params (string, object?)[] parametros) =>
        Convert.ToInt32(Escalar(sql, parametros));

    public static void Ejecutar(string sql, params (string Nombre, object? Valor)[] parametros)
    {
        using var conexion = Conexion.ObtenerConexionAbierta();
        using var comando = new SqlCommand(sql, conexion);
        foreach (var (nombre, valor) in parametros) comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        comando.ExecuteNonQuery();
    }

    /// <summary>Fuerza el vencimiento de la cuota de un socio (la fecha del seed es fija, no relativa a hoy).</summary>
    public static void FijarVencimiento(int idPersona, DateTime? fecha) =>
        Ejecutar("UPDATE dbo.Socio SET fecha_vencimiento_cuota = @f WHERE id_persona = @i", ("@f", fecha), ("@i", idPersona));

    public static Plan PlanPor(string periodicidad) =>
        new ServicioPlan().Listar().First(p => p.TipoPeriodicidad == periodicidad);

    public static int PrimerMedioPago() => new ServicioCatalogo().ObtenerMediosPago().First().IdMedioPago;
}
