using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio
{
    /// <summary>
    /// Punto único de búsqueda de Persona por documento, compartido entre el ABM de
    /// Socios y el de Usuarios (ambos pueden reutilizar una Persona ya cargada por
    /// el otro módulo en vez de duplicar sus datos personales).
    /// </summary>
    public class ServicioPersona
    {
        private readonly RepositorioPersona _repositorioPersona = new();

        public Persona? BuscarPorDocumento(string documento) =>
            _repositorioPersona.ObtenerPorDocumento(documento);
    }
}
