using System.Collections.Generic;
using System.Text.RegularExpressions;
using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio
{
    public class ServicioSocio
    {
        private readonly RepositorioSocio _repoSocio = new RepositorioSocio();
        private readonly RepositorioPersona _repoPersona = new RepositorioPersona();

        public int RegistrarSocio(Socio socio)
        {
            ValidarDatos(socio);

            // Validar si ya existe un socio activo con ese documento (RF#06)
            var socioExistente = _repoSocio.ObtenerPorDocumento(socio.IdTipoDocumento, socio.Documento);
            if (socioExistente != null && socioExistente.Activo)
            {
                throw new NegocioException("Ya existe un socio activo registrado con ese tipo y número de documento.");
            }

            // Reutilización de persona si ya existía en la BD
            var personaExistente = _repoPersona.ObtenerPorDocumento(socio.IdTipoDocumento, socio.Documento);
            if (personaExistente != null)
            {
                socio.IdPersona = personaExistente.IdPersona;
            }

            return _repoSocio.Guardar(socio);
        }

        public IEnumerable<Socio> ObtenerSocios()
        {
            return _repoSocio.ListarTodos();
        }

        public void DarDeBaja(int idSocio)
        {
            if (idSocio <= 0)
                throw new NegocioException("Identificador de socio inválido.");

            _repoSocio.BajaLogica(idSocio);
        }

        private void ValidarDatos(Socio socio)
        {
            if (socio == null)
                throw new NegocioException("Los datos del socio son requeridos.");

            if (string.IsNullOrWhiteSpace(socio.Nombre))
                throw new NegocioException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(socio.Apellido))
                throw new NegocioException("El apellido es obligatorio.");

            if (socio.IdTipoDocumento <= 0)
                throw new NegocioException("Debe seleccionar un tipo de documento válido.");

            // RF#06: Documento de 7 a 9 dígitos numéricos
            if (string.IsNullOrWhiteSpace(socio.Documento) || !Regex.IsMatch(socio.Documento.Trim(), @"^\d{7,9}$"))
                throw new NegocioException("El número de documento debe contener entre 7 y 9 dígitos numéricos.");

            // RF#09: Validación de formato de email con Regex si se ingresó
            if (!string.IsNullOrWhiteSpace(socio.Email))
            {
                const string regexEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(socio.Email.Trim(), regexEmail))
                    throw new NegocioException("El formato del correo electrónico es inválido.");
            }
        }
    }
}