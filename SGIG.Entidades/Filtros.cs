namespace SGIG.Entidades
{
    /// <summary>Estado (baja lógica) para filtrar listados de socios y usuarios.</summary>
    public enum FiltroEstado { Todos, Activos, DadosDeBaja }

    /// <summary>Situación de la cuota para filtrar socios (mismo criterio que el Check-in).</summary>
    public enum FiltroCuota { Todas, AlDia, PorVencer, Vencida, SinCuotas }

    /// <summary>Criterios de la barra de filtros de Socios (Fase 14).</summary>
    public class FiltroSocios
    {
        /// <summary>Busca en apellido, nombre o documento.</summary>
        public string? Texto { get; set; }
        public FiltroEstado Estado { get; set; } = FiltroEstado.Todos;
        public FiltroCuota Cuota { get; set; } = FiltroCuota.Todas;

        /// <summary>Plan preferido; null = todos.</summary>
        public int? IdPlan { get; set; }
    }

    /// <summary>Criterios de la barra de filtros de Usuarios (Fase 14).</summary>
    public class FiltroUsuarios
    {
        /// <summary>Busca en apellido, nombre, documento, legajo o nombre de usuario.</summary>
        public string? Texto { get; set; }
        public FiltroEstado Estado { get; set; } = FiltroEstado.Todos;

        /// <summary>Rol; null = todos.</summary>
        public int? IdRol { get; set; }
    }

    /// <summary>Criterios de la barra de filtros de Máquinas (Fase 14).</summary>
    public class FiltroMaquinas
    {
        /// <summary>Busca en nombre o marca.</summary>
        public string? Texto { get; set; }

        /// <summary>"Operativa", "En Reparacion" o null = todas.</summary>
        public string? Estado { get; set; }

        /// <summary>Marca exacta; null = todas.</summary>
        public string? Marca { get; set; }
    }
}
