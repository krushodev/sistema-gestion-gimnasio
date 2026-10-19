using System.Runtime.CompilerServices;

namespace SGIG.Tests;

/// <summary>
/// El host de pruebas (testhost) no lee el App.config de este proyecto: busca "testhost.dll.config".
/// Como <c>SGIG.Datos.Conexion</c> lee la cadena de conexión con ConfigurationManager, se le indica
/// explícitamente qué archivo usar antes de que nadie lo consulte. Así la cadena de pruebas
/// (base aislada SGIG_Test) rige igual con <c>dotnet test</c> que desde Visual Studio.
/// </summary>
internal static class ConfiguracionDePruebas
{
    [ModuleInitializer]
    internal static void Inicializar() =>
        AppContext.SetData("APP_CONFIG_FILE", Path.Combine(AppContext.BaseDirectory, "SGIG.Tests.dll.config"));
}
