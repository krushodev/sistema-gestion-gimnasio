using System.Text.RegularExpressions;

namespace SGIG.UI
{
    /// <summary>
    /// Validación de formato en la UI, previa a llamar a <c>SGIG.Negocio</c>: evita el
    /// viaje al servicio para errores que ya se detectan mirando el control (campo
    /// vacío, formato inválido), y marca el control con <see cref="ErrorProvider"/>
    /// para que el usuario vea exactamente qué corregir (RNF#04). No reemplaza las
    /// validaciones de <see cref="SGIG.Negocio.Validaciones"/>: esas son la última
    /// palabra (RNF#06) y siguen corriendo igual del lado del servicio.
    /// </summary>
    internal static partial class ValidacionesUI
    {
        /// <summary>Nombre/apellido: letras (con acentos), espacios y algunos signos habituales.</summary>
        [GeneratedRegex(@"^[\p{L}\s'.-]+$")]
        private static partial Regex RegexSoloLetras();

        public static bool EsSoloLetras(string texto) =>
            !string.IsNullOrWhiteSpace(texto) && RegexSoloLetras().IsMatch(texto.Trim());

        /// <summary>Restringe un TextBox a letras/espacios (RNF#04: nombre y apellido no admiten números).</summary>
        public static void SoloLetras(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) &&
                e.KeyChar != ' ' && e.KeyChar != '\'' && e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }

        /// <summary>Restringe un TextBox a dígitos y un único separador decimal (para montos).</summary>
        public static void SoloDecimal(object? sender, KeyPressEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            var esSeparador = e.KeyChar == '.' || e.KeyChar == ',';

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && !esSeparador)
            {
                e.Handled = true;
                return;
            }

            // Sólo un separador decimal por campo.
            if (esSeparador && (textBox.Text.Contains('.') || textBox.Text.Contains(',')))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Marca (o limpia) el error de <paramref name="control"/> en <paramref name="errorProvider"/>
        /// según <paramref name="esValido"/>. Devuelve el mismo valor, para poder encadenar
        /// varias validaciones con <c>&amp;</c> (no <c>&amp;&amp;</c>, para que se evalúen y marquen todas).
        /// </summary>
        public static bool Marcar(ErrorProvider errorProvider, Control control, bool esValido, string mensajeError)
        {
            errorProvider.SetError(control, esValido ? string.Empty : mensajeError);
            return esValido;
        }

        /// <summary>Limpia cualquier error previamente marcado sobre el control.</summary>
        public static void Limpiar(ErrorProvider errorProvider, Control control) =>
            errorProvider.SetError(control, string.Empty);
    }
}
