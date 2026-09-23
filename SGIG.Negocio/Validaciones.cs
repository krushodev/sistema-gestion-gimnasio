using System.Text.RegularExpressions;

namespace SGIG.Negocio
{
    /// <summary>
    /// Validaciones de formato con expresiones regulares (RF#09, RNF#04).
    /// Viven acá, en la capa de negocio, para que las compartan todos los ABM que
    /// cargan datos de Persona (Usuarios en la Fase 2, Socios en la Fase 3).
    /// </summary>
    public static partial class Validaciones
    {
        /// <summary>
        /// id_tipo_documento del DNI en el seed de dbo.TipoDocumento (ver sección 9
        /// de docs/SGIG_CreateDB.sql: DNI=1, Pasaporte=2, Cédula=3). Sólo al DNI se
        /// le exige el formato numérico de 7-8 dígitos de un documento argentino
        /// vigente; Pasaporte y Cédula pueden incluir letras.
        /// </summary>
        public const int IdTipoDocumentoDni = 1;

        /// <summary>
        /// Documento genérico (Pasaporte/Cédula): alfanumérico, entre 6 y 20
        /// caracteres (la columna es VARCHAR(20)).
        /// </summary>
        [GeneratedRegex(@"^[A-Za-z0-9]{6,20}$")]
        private static partial Regex RegexDocumentoGenerico();

        /// <summary>DNI argentino: sólo dígitos, 7 u 8 (nunca menos, nunca más).</summary>
        [GeneratedRegex(@"^\d{7,8}$")]
        private static partial Regex RegexDni();

        /// <summary>
        /// Email con la estructura habitual usuario@dominio.tld. No pretende cubrir
        /// el RFC completo: alcanza para atajar los errores de tipeo reales.
        /// </summary>
        [GeneratedRegex(@"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)+$")]
        private static partial Regex RegexEmail();

        /// <summary>Teléfono: dígitos, espacios, guiones, paréntesis y un + inicial.</summary>
        [GeneratedRegex(@"^\+?[\d\s\-()]{6,30}$")]
        private static partial Regex RegexTelefono();

        /// <summary>
        /// Documento válido para un tipo de documento cualquiera: DNI exige 7-8
        /// dígitos numéricos; Pasaporte/Cédula aceptan alfanumérico de 6-20.
        /// </summary>
        public static bool EsDocumentoValido(string documento, int idTipoDocumento)
        {
            if (string.IsNullOrWhiteSpace(documento)) return false;
            var valor = documento.Trim();

            return idTipoDocumento == IdTipoDocumentoDni
                ? RegexDni().IsMatch(valor)
                : RegexDocumentoGenerico().IsMatch(valor);
        }

        public static bool EsEmailValido(string email) =>
            !string.IsNullOrWhiteSpace(email) && RegexEmail().IsMatch(email.Trim());

        public static bool EsTelefonoValido(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono)) return false;
            var valor = telefono.Trim();

            if (!RegexTelefono().IsMatch(valor)) return false;

            // Un teléfono argentino real tiene entre 8 dígitos (fijo local) y 13
            // (celular con característica internacional +54 9 + área + número).
            var soloDigitos = new string(valor.Where(char.IsDigit).ToArray());
            return soloDigitos.Length is >= 8 and <= 13;
        }

        /// <summary>
        /// Valida los datos personales comunes a cualquier Persona. El email y el
        /// teléfono son opcionales en la base: sólo se validan si vienen cargados.
        /// </summary>
        public static void ValidarDatosDePersona(string documento, int idTipoDocumento, string? email, string? telefono)
        {
            if (!EsDocumentoValido(documento, idTipoDocumento))
            {
                var mensaje = idTipoDocumento == IdTipoDocumentoDni
                    ? "El DNI debe tener sólo números, 7 u 8 dígitos."
                    : "El documento debe ser alfanumérico, entre 6 y 20 caracteres.";
                throw new NegocioException(mensaje);
            }

            if (!string.IsNullOrWhiteSpace(email) && !EsEmailValido(email))
            {
                throw new NegocioException(
                    $"El email \"{email}\" no tiene un formato válido. Debe ser del estilo nombre@dominio.com.");
            }

            if (!string.IsNullOrWhiteSpace(telefono) && !EsTelefonoValido(telefono))
            {
                throw new NegocioException(
                    "El teléfono no es válido: usá sólo números, espacios, guiones y paréntesis, " +
                    "con un total de 8 a 13 dígitos.");
            }
        }
    }
}
