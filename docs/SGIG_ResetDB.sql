/* ============================================================
   Sistema de Gestion Integral para Gimnasios (SGIG)
   Script de reseteo de base de datos - SQL Server
   ============================================================
   Elimina la base SGIG por completo (todas las tablas y datos).
   Usar solo en entornos de desarrollo/pruebas.

   Despues de correr este script, volver a ejecutar
   SGIG_CreateDB.sql para recrear el esquema y los seeders
   (catalogos + usuario admin/admin1234).

   No es necesario tocar SGIG_CreateDB.sql: ese script ya
   contempla crear la base si no existe (IF DB_ID('SGIG') IS
   NULL), por eso el reseteo completo es DROP + volver a correr
   ese mismo script, no un script de creacion aparte.
   ============================================================ */

USE master;
GO

IF DB_ID('SGIG') IS NOT NULL
BEGIN
    ALTER DATABASE SGIG SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SGIG;
END
GO
