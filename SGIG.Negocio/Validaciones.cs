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
        /// Documento: sólo dígitos, entre 6 y 20 (la columna es VARCHAR(20)).
        /// Deja afuera cualquier letra, que es justo lo que pide el RNF#04.
        /// </summary>
        [GeneratedRegex(@"^\d{6,20}$")]
        private static partial Regex RegexDocumento();

        /// <summary>
        /// Email con la estructura habitual usuario@dominio.tld. No pretende cubrir
        /// el RFC completo: alcanza para atajar los errores de tipeo reales.
        /// </summary>
        [GeneratedRegex(@"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)+$")]
        private static partial Regex RegexEmail();

        /// <summary>Teléfono: dígitos, espacios, guiones, paréntesis y un + inicial.</summary>
        [GeneratedRegex(@"^\+?[\d\s\-()]{6,30}$")]
        private static partial Regex RegexTelefono();

        public static bool EsDocumentoValido(string documento) =>
            !string.IsNullOrWhiteSpace(documento) && RegexDocumento().IsMatch(documento.Trim());

        public static bool EsEmailValido(string email) =>
            !string.IsNullOrWhiteSpace(email) && RegexEmail().IsMatch(email.Trim());

        public static bool EsTelefonoValido(string telefono) =>
            !string.IsNullOrWhiteSpace(telefono) && RegexTelefono().IsMatch(telefono.Trim());

        /// <summary>
        /// Valida los datos personales comunes a cualquier Persona. El email y el
        /// teléfono son opcionales en la base: sólo se validan si vienen cargados.
        /// </summary>
        public static void ValidarDatosDePersona(string documento, string? email, string? telefono)
        {
            if (!EsDocumentoValido(documento))
            {
                throw new NegocioException(
                    "El documento debe tener sólo números (entre 6 y 20 dígitos), sin letras ni puntos.");
            }

            if (!string.IsNullOrWhiteSpace(email) && !EsEmailValido(email))
            {
                throw new NegocioException(
                    $"El email \"{email}\" no tiene un formato válido. Debe ser del estilo nombre@dominio.com.");
            }

            if (!string.IsNullOrWhiteSpace(telefono) && !EsTelefonoValido(telefono))
            {
                throw new NegocioException(
                    "El teléfono sólo admite números, espacios, guiones y paréntesis.");
            }
        }
    }
}
