namespace SGIG.UI
{
    /// <summary>
    /// Configuración de columnas para los <see cref="DataGridView"/> del sistema.
    /// </summary>
    /// <remarks>
    /// Por defecto un DataGridView enlazado a una lista genera una columna por cada
    /// propiedad pública de la entidad, lo que expone datos internos (ids, banderas,
    /// el hash de la contraseña) con encabezados feos. Todas las grillas del sistema
    /// declaran sus columnas explícitamente con este helper.
    /// </remarks>
    internal static class Grillas
    {
        /// <summary>
        /// Reemplaza las columnas autogeneradas por la lista indicada, en orden.
        /// Cada columna se declara como (propiedad de la entidad, encabezado visible,
        /// peso relativo de ancho).
        /// </summary>
        public static void Configurar(
            DataGridView grilla,
            params (string Propiedad, string Encabezado, int Peso)[] columnas)
        {
            grilla.AutoGenerateColumns = false;
            grilla.Columns.Clear();
            grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            foreach (var (propiedad, encabezado, peso) in columnas)
            {
                grilla.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "col" + propiedad,
                    DataPropertyName = propiedad,
                    HeaderText = encabezado,
                    FillWeight = peso,
                    SortMode = DataGridViewColumnSortMode.Automatic
                });
            }
        }

        /// <summary>
        /// Sólo admite dígitos en un TextBox (RNF#04: los campos numéricos no deben
        /// aceptar letras). Se engancha al evento <c>KeyPress</c> del control.
        /// </summary>
        public static void SoloDigitos(object? sender, KeyPressEventArgs e)
        {
            // Se permiten los dígitos y las teclas de control (Backspace, Supr).
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
