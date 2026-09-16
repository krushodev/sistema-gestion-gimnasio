using System.Collections.Generic;
using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio;

/// <summary>
/// Plan es un catálogo sembrado con la aplicación (ver docs/SGIG_CreateDB.sql) y
/// no tiene ABM en la UI; este servicio solo expone lectura.
/// </summary>
public class ServicioPlan
{
    private readonly RepositorioPlan _repositorioPlan = new();

    public IEnumerable<Plan> Listar(bool soloActivos = true)
    {
        return _repositorioPlan.ObtenerTodos(soloActivos);
    }

    public Plan? ObtenerPorId(int idPlan)
    {
        return _repositorioPlan.ObtenerPorId(idPlan);
    }
}
