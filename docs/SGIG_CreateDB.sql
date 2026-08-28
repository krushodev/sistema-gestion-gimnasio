/* ============================================================
   Sistema de Gestion Integral para Gimnasios (SGIG)
   Script de creacion de base de datos - SQL Server
   Basado en el DER v5 (14 tablas) - version 3.0 de la ERS
   ============================================================
   Orden de creacion: primero las tablas sin dependencias
   (parametricas), luego Persona, luego sus especializaciones
   (Socio, Usuario), y por ultimo las tablas transaccionales
   que dependen de las anteriores.
   ============================================================ */

IF DB_ID('SGIG') IS NULL
BEGIN
    CREATE DATABASE SGIG;
END
GO

USE SGIG;
GO

/* ============================================================
   1. TABLAS PARAMETRICAS (sin dependencias)
   ------------------------------------------------------------
   Todas llevan el campo 'activo' porque su baja es LOGICA
   (RF#04, v3.2 de la ERS): nunca se borra fisicamente un
   catalogo, para no romper los registros historicos que lo
   referencian. Las consultas de la aplicacion filtran activo=1.
   ============================================================ */

CREATE TABLE dbo.Provincia (
    id_provincia    INT IDENTITY(1,1) PRIMARY KEY,
    nombre          VARCHAR(100) NOT NULL,
    activo          BIT NOT NULL DEFAULT 1
);

CREATE TABLE dbo.TipoDocumento (
    id_tipo_documento  INT IDENTITY(1,1) PRIMARY KEY,
    descripcion        VARCHAR(50) NOT NULL,
    activo             BIT NOT NULL DEFAULT 1
);

CREATE TABLE dbo.Rol (
    id_rol       INT IDENTITY(1,1) PRIMARY KEY,
    nombre_rol   VARCHAR(50) NOT NULL,
    descripcion  VARCHAR(200) NULL,
    activo       BIT NOT NULL DEFAULT 1
);

CREATE TABLE dbo.MedioPago (
    id_medio_pago  INT IDENTITY(1,1) PRIMARY KEY,
    descripcion    VARCHAR(50) NOT NULL,
    activo         BIT NOT NULL DEFAULT 1
);

CREATE TABLE dbo.[Plan] (
    id_plan        INT IDENTITY(1,1) PRIMARY KEY,
    nombre         VARCHAR(100) NOT NULL,
    precio         DECIMAL(10,2) NOT NULL,
    dias_vigencia  INT NOT NULL,
    activo         BIT NOT NULL DEFAULT 1
);

/* ============================================================
   2. LOCALIDAD (depende de Provincia)
   ============================================================ */

CREATE TABLE dbo.Localidad (
    id_localidad  INT IDENTITY(1,1) PRIMARY KEY,
    nombre        VARCHAR(100) NOT NULL,
    id_provincia  INT NOT NULL,
    activo        BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Localidad_Provincia
        FOREIGN KEY (id_provincia) REFERENCES dbo.Provincia(id_provincia)
);

/* ============================================================
   3. PERSONA (supertipo - depende de TipoDocumento y Localidad)
   ============================================================ */

CREATE TABLE dbo.Persona (
    id_persona          INT IDENTITY(1,1) PRIMARY KEY,
    documento           VARCHAR(20) NOT NULL,
    id_tipo_documento   INT NOT NULL,
    nombre              VARCHAR(100) NOT NULL,
    apellido            VARCHAR(100) NOT NULL,
    email               VARCHAR(150) NULL,
    telefono            VARCHAR(30) NULL,
    id_localidad        INT NULL,
    fecha_nacimiento    DATE NULL,
    CONSTRAINT UQ_Persona_Documento UNIQUE (documento),
    CONSTRAINT FK_Persona_TipoDocumento
        FOREIGN KEY (id_tipo_documento) REFERENCES dbo.TipoDocumento(id_tipo_documento),
    CONSTRAINT FK_Persona_Localidad
        FOREIGN KEY (id_localidad) REFERENCES dbo.Localidad(id_localidad)
);

/* ============================================================
   4. ESPECIALIZACIONES DE PERSONA: Socio y Usuario
   (patron "tabla por subtipo": comparten id_persona como PK y FK)
   ============================================================ */

CREATE TABLE dbo.Socio (
    id_persona                INT PRIMARY KEY,
    apto_medico               VARCHAR(200) NULL,
    id_plan                   INT NULL,
    fecha_vencimiento_cuota   DATE NULL,
    activo                    BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Socio_Persona
        FOREIGN KEY (id_persona) REFERENCES dbo.Persona(id_persona),
    CONSTRAINT FK_Socio_Plan
        FOREIGN KEY (id_plan) REFERENCES dbo.[Plan](id_plan)
);

CREATE TABLE dbo.Usuario (
    id_persona          INT PRIMARY KEY,
    nombre_usuario       VARCHAR(50) NOT NULL,
    contrasenia_hash    VARBINARY(256) NOT NULL,
    id_rol              INT NOT NULL,
    legajo              VARCHAR(20) NOT NULL,
    fecha_ingreso       DATE NULL,
    activo              BIT NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Usuario_NombreUsuario UNIQUE (nombre_usuario),
    CONSTRAINT UQ_Usuario_Legajo UNIQUE (legajo),
    CONSTRAINT FK_Usuario_Persona
        FOREIGN KEY (id_persona) REFERENCES dbo.Persona(id_persona),
    CONSTRAINT FK_Usuario_Rol
        FOREIGN KEY (id_rol) REFERENCES dbo.Rol(id_rol)
);

/* ============================================================
   5. TESORERIA (Pago depende de Socio, Plan y MedioPago)
   ============================================================ */

CREATE TABLE dbo.Pago (
    id_pago                       INT IDENTITY(1,1) PRIMARY KEY,
    id_persona                    INT NOT NULL,
    id_plan                       INT NOT NULL,
    id_medio_pago                 INT NOT NULL,
    fecha_pago                    DATE NOT NULL,
    monto                         DECIMAL(10,2) NOT NULL,
    fecha_vencimiento_generada    DATE NULL,
    CONSTRAINT FK_Pago_Socio
        FOREIGN KEY (id_persona) REFERENCES dbo.Socio(id_persona),
    CONSTRAINT FK_Pago_Plan
        FOREIGN KEY (id_plan) REFERENCES dbo.[Plan](id_plan),
    CONSTRAINT FK_Pago_MedioPago
        FOREIGN KEY (id_medio_pago) REFERENCES dbo.MedioPago(id_medio_pago)
);

/* ============================================================
   6. CONTROL DE ACCESO (Checkin depende de Socio)
   ============================================================ */

CREATE TABLE dbo.Checkin (
    id_checkin   INT IDENTITY(1,1) PRIMARY KEY,
    id_persona   INT NOT NULL,
    fecha_hora   DATETIME NOT NULL DEFAULT GETDATE(),
    resultado    VARCHAR(20) NOT NULL,
    CONSTRAINT FK_Checkin_Socio
        FOREIGN KEY (id_persona) REFERENCES dbo.Socio(id_persona),
    CONSTRAINT CK_Checkin_Resultado
        CHECK (resultado IN ('Concedido', 'Rechazado'))
);

/* ============================================================
   7. ACTIVOS Y GASTOS
   ============================================================ */

CREATE TABLE dbo.Maquina (
    id_maquina     INT IDENTITY(1,1) PRIMARY KEY,
    marca          VARCHAR(50) NULL,
    nombre         VARCHAR(100) NOT NULL,
    fecha_compra   DATE NULL,
    estado         VARCHAR(20) NOT NULL DEFAULT 'Operativa',
    CONSTRAINT CK_Maquina_Estado
        CHECK (estado IN ('Operativa', 'En Reparacion'))
);

CREATE TABLE dbo.Gasto (
    id_gasto      INT IDENTITY(1,1) PRIMARY KEY,
    fecha         DATE NOT NULL,
    monto         DECIMAL(10,2) NOT NULL,
    descripcion   VARCHAR(300) NULL,
    comprobante   VARCHAR(100) NULL
);

/* Mantenimiento depende de Maquina, Usuario (tecnico) y Gasto */
CREATE TABLE dbo.Mantenimiento (
    id_mantenimiento   INT IDENTITY(1,1) PRIMARY KEY,
    id_maquina         INT NOT NULL,
    id_persona         INT NOT NULL,   -- tecnico a cargo (usuario con rol Tecnico)
    fecha_inicio       DATE NOT NULL,
    fecha_fin          DATE NULL,
    detalle_tecnico    VARCHAR(500) NULL,
    id_gasto           INT NOT NULL,
    CONSTRAINT UQ_Mantenimiento_Gasto UNIQUE (id_gasto),
    CONSTRAINT FK_Mantenimiento_Maquina
        FOREIGN KEY (id_maquina) REFERENCES dbo.Maquina(id_maquina),
    CONSTRAINT FK_Mantenimiento_Usuario
        FOREIGN KEY (id_persona) REFERENCES dbo.Usuario(id_persona),
    CONSTRAINT FK_Mantenimiento_Gasto
        FOREIGN KEY (id_gasto) REFERENCES dbo.Gasto(id_gasto)
);

/* ============================================================
   8. INDICES ADICIONALES (para las consultas mas frecuentes)
   ============================================================
   Las columnas UNIQUE (documento, nombre_usuario, legajo)
   ya generan su propio indice automaticamente. Se agregan
   indices sobre las FK mas consultadas para acelerar el
   Check-in y el historial de pagos.
   ============================================================ */

CREATE INDEX IX_Checkin_Persona_Fecha
    ON dbo.Checkin (id_persona, fecha_hora DESC);

CREATE INDEX IX_Pago_Persona_Fecha
    ON dbo.Pago (id_persona, fecha_pago DESC);

CREATE INDEX IX_Mantenimiento_Maquina
    ON dbo.Mantenimiento (id_maquina);

CREATE INDEX IX_Gasto_Fecha
    ON dbo.Gasto (fecha);

GO

/* ============================================================
   9. DATOS INICIALES (seed) - tablas parametricas
   Necesarios para poder probar el sistema desde el primer dia:
   sin Rol y sin un Usuario Administrador, no se puede ni
   iniciar sesion.
   ============================================================ */

INSERT INTO dbo.Rol (nombre_rol, descripcion) VALUES
    ('Administrador', 'Control total del sistema'),
    ('Recepcionista', 'Atencion al publico: socios, cuotas, check-in'),
    ('Tecnico', 'Gestion de maquinas y mantenimientos');

INSERT INTO dbo.TipoDocumento (descripcion) VALUES
    ('DNI'),
    ('Pasaporte'),
    ('Cedula');

INSERT INTO dbo.MedioPago (descripcion) VALUES
    ('Efectivo'),
    ('Tarjeta'),
    ('Transferencia');

/* Persona + Usuario administrador inicial.
   contrasenia_hash es el SHA256 real de la contraseña 'admin', calculado con
   System.Security.Cryptography (SGIG.Negocio.Hash.Calcular). Son 32 bytes; la
   contraseña nunca se guarda en texto plano (RNF#11).
   CAMBIAR esta contraseña desde el ABM de usuarios apenas se instale el sistema. */

INSERT INTO dbo.Persona (documento, id_tipo_documento, nombre, apellido, email)
VALUES ('00000001', 1, 'Admin', 'Sistema', 'admin@sgig.local');

INSERT INTO dbo.Usuario (id_persona, nombre_usuario, contrasenia_hash, id_rol, legajo, fecha_ingreso)
VALUES (
    SCOPE_IDENTITY(),
    'admin',
    0x8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918, -- SHA256 de 'admin'
    (SELECT id_rol FROM dbo.Rol WHERE nombre_rol = 'Administrador'),
    'LEG-0001',
    GETDATE()
);

USE [master];
CREATE LOGIN sgig_user WITH PASSWORD = 'admin', CHECK_POLICY = OFF;

USE SGIG;
CREATE USER sgig_user FOR LOGIN sgig_user;
ALTER ROLE db_owner ADD MEMBER sgig_user;

GO
