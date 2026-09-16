using System.Collections.Generic;
using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio;

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

    public void Guardar(Plan plan)
    {
        Validar(plan);

        if (plan.IdPlan == 0)
            _repositorioPlan.Insertar(plan);
        else
            _repositorioPlan.Modificar(plan);
    }

    public void CambiarEstado(int idPlan, bool activo)
    {
        _repositorioPlan.CambiarEstado(idPlan, activo);
    }

    private static void Validar(Plan plan)
    {
        if (string.IsNullOrWhiteSpace(plan.Nombre))
            throw new NegocioException("El nombre del plan es obligatorio.");

        if (plan.Precio <= 0)
            throw new NegocioException("El precio debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(plan.TipoPeriodicidad))
            throw new NegocioException("La periodicidad es obligatoria.");
    }
}