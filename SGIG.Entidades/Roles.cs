namespace SGIG.Entidades
{
    /// <summary>
    /// Constantes de IdRol para los 3 roles fijos del sistema (Administrador,
    /// Recepcionista, Técnico). Los roles no se crean dinámicamente desde la UI de
    /// negocio; estos valores dependen del orden de INSERT del seed en
    /// docs/SGIG_CreateDB.sql (IDENTITY 1, 2, 3 en ese orden). Se usan para no
    /// comparar el nombre del rol como string en frmMDIParent.
    /// </summary>
    public static class Roles
    {
        public const int Administrador = 1;
        public const int Recepcionista = 2;
        public const int Tecnico = 3;
    }
}
