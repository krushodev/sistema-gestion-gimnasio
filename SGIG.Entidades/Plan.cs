namespace SGIG.Entidades;

public class Plan
{
    public int IdPlan { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string TipoPeriodicidad { get; set; } = "Mensual";
    public bool Activo { get; set; }
}