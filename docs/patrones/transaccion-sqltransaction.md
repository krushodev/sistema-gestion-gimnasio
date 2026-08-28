# Patrón: transacción con `SqlTransaction` + Dapper

Toda operación que escriba en **más de una tabla** va dentro de una transacción explícita:
alta de `Persona` + `Socio`, alta de `Persona` + `Usuario`, y sobre todo el alta de
`Mantenimiento`, que además genera el `Gasto` asociado y cambia el estado de la `Maquina`
en el mismo commit (RF#20, RF#23).

Dapper no gestiona la transacción: se abre igual que con ADO.NET puro y se pasa **como
tercer argumento** a cada `Execute` / `ExecuteScalar<T>` / `Query<T>`. Si no se pasa, esa
sentencia queda fuera de la transacción y el rollback no la deshace.

```csharp
using var connection = Conexion.ObtenerConexionAbierta();
using var transaction = connection.BeginTransaction();
try
{
    var idMantenimiento = connection.ExecuteScalar<int>(sqlInsertMantenimiento, parametros, transaction);
    connection.Execute(sqlUpdateEstadoMaquina, parametrosMaquina, transaction);
    connection.Execute(sqlInsertGasto, parametrosGasto, transaction);

    transaction.Commit();
    return idMantenimiento;
}
catch (SqlException ex)
{
    transaction.Rollback();
    throw new AccesoDatosException("No se pudo registrar el mantenimiento.", ex);
}
```

Puntos a no olvidar:

- `using` en la conexión **y** en la transacción — el `Dispose` de una transacción sin commit hace rollback.
- El `Rollback()` va en el `catch`, antes de envolver y relanzar como `AccesoDatosException`.
- Para recuperar el id recién insertado: `... ; SELECT CAST(SCOPE_IDENTITY() AS int);` con `ExecuteScalar<int>`.
- La orquestación puede vivir en el repositorio (una entidad y su especialización) o en el
  servicio de `SGIG.Negocio` cuando cruza varios repositorios; en ese caso el servicio abre la
  conexión y la transacción y se las pasa a los repositorios.

Ver también: [repository-dapper.md](repository-dapper.md).
