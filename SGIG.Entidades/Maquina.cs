using System;

namespace SGIG.Entidades
{
    /// <summary>Máquina/equipo del gimnasio (RF#18). No tiene baja lógica: dbo.Maquina no tiene columna 'activo'.</summary>
    public class Maquina
    {
        public int IdMaquina { get; set; }
        public string? Marca { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime? FechaCompra { get; set; }

        /// <summary>"Operativa" o "En Reparacion" (CK_Maquina_Estado, sin tilde).</summary>
        public string Estado { get; set; } = "Operativa";
    }
}
