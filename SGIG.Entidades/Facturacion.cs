using System;

namespace SGIG.Entidades;

public class Facturacion
{
    public int IdFacturacion { get; set; }
    public int IdPersona { get; set; }
    public int IdPlan { get; set; }
    public DateTime FechaEmision { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public decimal MontoTotal { get; set; }
    public string Estado { get; set; } = "Pagada";
}