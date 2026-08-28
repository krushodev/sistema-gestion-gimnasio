namespace SGIG.UI
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada de la aplicación.
        /// El primer formulario del sistema es frmLogin (Fase 1.1); recién tras un login
        /// exitoso se abre frmMDIParent, contenedor único de todas las pantallas (RNF#02).
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Configuración de alto DPI y fuente por defecto: https://aka.ms/applicationconfiguration
            ApplicationConfiguration.Initialize();

            // TODO (Fase 1.1): descomentar cuando frmLogin exista.
            // Application.Run(new frmLogin());
        }
    }
}
