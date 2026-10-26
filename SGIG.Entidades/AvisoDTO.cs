namespace SGIG.Entidades
{
    /// <summary>
    /// Un aviso del bloque "Para atender hoy" del Inicio (Fase 13), armado por
    /// <c>ServicioAvisos</c>. La UI elige color e ícono por <see cref="Nivel"/> y a qué
    /// pantalla lleva por <see cref="Categoria"/>.
    /// </summary>
    public class AvisoDTO
    {
        /// <summary>"Peligro", "Aviso", "Info" u "Ok".</summary>
        public string Nivel { get; set; } = "Info";

        public string Texto { get; set; } = string.Empty;

        /// <summary>"Cuotas", "Maquinas", "Cumpleanios" o "General".</summary>
        public string Categoria { get; set; } = "General";
    }
}
