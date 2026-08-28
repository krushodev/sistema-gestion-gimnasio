namespace SGIG.Entidades
{
    /// <summary>Medio de pago de una cuota: Efectivo, Tarjeta, Transferencia.</summary>
    public class MedioPago
    {
        public int IdMedioPago { get; set; }
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>Baja lógica (RF#04): nunca se borra físicamente un catálogo.</summary>
        public bool Activo { get; set; } = true;
    }
}
