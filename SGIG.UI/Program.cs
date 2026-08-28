namespace SGIG.UI
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada de la aplicación. Mantiene el ciclo login ↔ sistema:
        /// se piden credenciales en <see cref="frmLogin"/> y, si son válidas, se abre
        /// <see cref="frmMDIParent"/>, contenedor único de todas las pantallas (RNF#02).
        /// Al cerrar sesión el contenedor se cierra y se vuelve a pedir el login;
        /// cancelar el login termina la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Configuración de alto DPI y fuente por defecto: https://aka.ms/applicationconfiguration
            ApplicationConfiguration.Initialize();

            while (true)
            {
                using var login = new frmLogin();

                if (login.ShowDialog() != DialogResult.OK)
                {
                    break;
                }

                Application.Run(new frmMDIParent(login.UsuarioAutenticado!));
            }
        }
    }
}
