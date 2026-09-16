using System;

namespace SGIG.Entidades;

public class Pago
{
    public int IdPago { get; set; }
    public int IdFacturacion { get; set; }
    public int IdMedioPago { get; set; }
    public DateTime FechaPago { get; set; }
    public decimal Monto { get; set; }

    // Propiedades calculadas / navegación para la grilla
    public string PlanNombre { get; set; } = string.Empty;
    public string MedioPagoDesc { get; set; } = string.Empty;
    public DateTime FechaVencimiento { get; set; }
}