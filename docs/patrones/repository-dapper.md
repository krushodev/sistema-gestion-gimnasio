# Patrón: Repositorio con Dapper (capa `SGIG.Datos`)

Un repositorio por entidad, en `SGIG.Datos`. Reglas fijas:

1. **SQL explícito** escrito a mano en una `const string`, nunca generado por el ORM.
2. **Parámetros siempre** vía objeto anónimo — jamás concatenar ni interpolar (`$"..."`) datos de usuario.
3. **Alias `AS`** en cada columna cuyo nombre snake_case no coincida con la propiedad PascalCase de la entidad (Dapper mapea por nombre).
4. **`Try…Catch`** capturando `SqlException` (de `Microsoft.Data.SqlClient`) y relanzando `AccesoDatosException` (RNF#06).
5. **Devuelve entidades** de `SGIG.Entidades` o `IEnumerable<T>` — nunca `DataTable`, `DataSet` ni `dynamic`.
6. La conexión sale siempre de `Conexion.ObtenerConexionAbierta()` y se libera con `using`.

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    public class RepositorioSocio
    {
        public Socio? ObtenerPorDocumento(string documento)
        {
            const string sql = @"
                SELECT p.id_persona AS IdPersona, p.documento AS Documento, p.nombre AS Nombre,
                       s.activo AS Activo, s.fecha_vencimiento_cuota AS FechaVencimientoCuota
                FROM dbo.Persona p
                INNER JOIN dbo.Socio s ON s.id_persona = p.id_persona
                WHERE p.documento = @Documento";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.QuerySingleOrDefault<Socio>(sql, new { Documento = documento });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al buscar el socio por documento.", ex);
            }
        }
    }
}
```

**Métodos típicos:** `ObtenerPorId(int idPersona)`, `ObtenerActivos()` → `IEnumerable<Socio>`, `Alta(Socio socio)` → `int` (con `ExecuteScalar<int>` + `SCOPE_IDENTITY()`), `Modificar(Socio socio)`, `BajaLogica(int idPersona)`.

**Baja lógica, nunca `DELETE`** sobre `Socio`, `Usuario` y `Plan` (RNF#03): el repositorio hace `UPDATE ... SET activo = 0`.

Ver también: [transaccion-sqltransaction.md](transaccion-sqltransaction.md).
