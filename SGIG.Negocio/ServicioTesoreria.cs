using System;
using System.Collections.Generic;
using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio;

public class ServicioTesoreria
{
    private readonly RepositorioTesoreria _repositorioTesoreria = new();
    private readonly RepositorioPlan _repositorioPlan = new();

    public IEnumerable<Pago> ObtenerHistorialSocio(int idPersona)
    {
        return _repositorioTesoreria.ObtenerHistorialPorSocio(idPersona);
    }

    // RF#11 y RF#12: Registro de Facturación, Pago y cálculo por calendario
    public void RegistrarCobro(int idPersona, int idPlan, int idMedioPago, DateTime? vencimientoActual)
    {
        var plan = _repositorioPlan.ObtenerPorId(idPlan)
            ?? throw new NegocioException("El plan seleccionado no existe o no se encuentra disponible.");

        DateTime emision = DateTime.Today;
        DateTime baseCalculo = (vencimientoActual.HasValue && vencimientoActual.Value > emision)
            ? vencimientoActual.Value
            : emision;

        DateTime nuevoVencimiento = plan.TipoPeriodicidad switch
        {
            "Diario" => baseCalculo.AddDays(1),
            "Semanal" => baseCalculo.AddDays(7),
            "Mensual" => baseCalculo.AddMonths(1),
            "Anual" => baseCalculo.AddYears(1),
            _ => baseCalculo.AddMonths(1)
        };

        var facturacion = new Facturacion
        {
            IdPersona = idPersona,
            IdPlan = idPlan,
            FechaEmision = emision,
            FechaVencimiento = nuevoVencimiento,
            MontoTotal = plan.Precio,
            Estado = "Pagada"
        };

        var pago = new Pago
        {
            IdMedioPago = idMedioPago,
            FechaPago = DateTime.Now,
            Monto = plan.Precio
        };

        _repositorioTesoreria.RegistrarCobroTransaccional(facturacion, pago, nuevoVencimiento);
    }
}