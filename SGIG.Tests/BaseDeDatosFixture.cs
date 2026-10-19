using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using SGIG.Datos;

namespace SGIG.Tests;

/// <summary>
/// Crea una base de datos AISLADA (<c>SGIG_Test</c>) a partir de <c>docs/SGIG_CreateDB.sql</c>
/// al empezar la corrida y la borra al terminar. Todas las pruebas de integración la
/// comparten (ver <see cref="ColeccionBaseDeDatos"/>), así que cada prueba debe crear sus
/// propios datos con documentos únicos (<see cref="Datos.Doc"/>) y no depender de las demás.
///
/// Guarda de seguridad: si la cadena de conexión de pruebas no apunta a "SGIG_Test" se
/// aborta antes de ejecutar nada, para no tocar nunca la base real.
/// </summary>
public sealed class BaseDeDatosFixture : IAsyncLifetime
{
    public const string NombreBase = "SGIG_Test";

    public async Task InitializeAsync()
    {
        var cadena = new SqlConnectionStringBuilder(Conexion.ObtenerCadena());
        if (!string.Equals(cadena.InitialCatalog, NombreBase, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Las pruebas sólo corren sobre la base '{NombreBase}', pero la cadena de conexión apunta a " +
                $"'{cadena.InitialCatalog}'. Revisá SGIG.Tests/App.config.");
        }

        var master = new SqlConnectionStringBuilder(cadena.ConnectionString) { InitialCatalog = "master" };
        await using var conexion = new SqlConnection(master.ConnectionString);
        await conexion.OpenAsync();

        await Ejecutar(conexion, SqlEliminarBase);

        // El script crea "SGIG": se reemplaza el nombre (sólo en CREATE/USE/DB_ID) por el de pruebas.
        var script = File.ReadAllText(RutaScript())
            .Replace("DB_ID('SGIG')", $"DB_ID('{NombreBase}')")
            .Replace("CREATE DATABASE SGIG;", $"CREATE DATABASE {NombreBase};")
            .Replace("USE SGIG;", $"USE {NombreBase};");

        foreach (var lote in Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(lote)) await Ejecutar(conexion, lote);
        }
    }

    public async Task DisposeAsync()
    {
        SqlConnection.ClearAllPools();
        var master = new SqlConnectionStringBuilder(Conexion.ObtenerCadena()) { InitialCatalog = "master" };
        await using var conexion = new SqlConnection(master.ConnectionString);
        await conexion.OpenAsync();
        await Ejecutar(conexion, SqlEliminarBase);
    }

    private static readonly string SqlEliminarBase = $@"
        IF DB_ID('{NombreBase}') IS NOT NULL
        BEGIN
            ALTER DATABASE [{NombreBase}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            DROP DATABASE [{NombreBase}];
        END";

    private static async Task Ejecutar(SqlConnection conexion, string sql)
    {
        await using var comando = new SqlCommand(sql, conexion) { CommandTimeout = 180 };
        await comando.ExecuteNonQueryAsync();
    }

    /// <summary>Sube desde el directorio de salida hasta encontrar docs/SGIG_CreateDB.sql.</summary>
    private static string RutaScript()
    {
        var directorio = new DirectoryInfo(AppContext.BaseDirectory);
        while (directorio != null)
        {
            var candidato = Path.Combine(directorio.FullName, "docs", "SGIG_CreateDB.sql");
            if (File.Exists(candidato)) return candidato;
            directorio = directorio.Parent;
        }
        throw new FileNotFoundException("No se encontró docs/SGIG_CreateDB.sql subiendo desde " + AppContext.BaseDirectory);
    }
}

/// <summary>Todas las clases con [Collection("BaseDeDatos")] comparten una sola base y corren en serie.</summary>
[CollectionDefinition(Nombre)]
public sealed class ColeccionBaseDeDatos : ICollectionFixture<BaseDeDatosFixture>
{
    public const string Nombre = "BaseDeDatos";
}
