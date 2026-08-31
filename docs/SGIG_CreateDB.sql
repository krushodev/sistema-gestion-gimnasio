/* ============================================================
   Sistema de Gestion Integral para Gimnasios (SGIG)
   Script de creacion de base de datos - SQL Server
   Basado en el DER v6 (14 tablas) - version 4.0 de la ERS
   ============================================================
   Cambios respecto del DER v5:
   - Se elimina la tabla Gasto. Mantenimiento ya no genera un
     gasto asociado (RF#23 dado de baja).
   - Plan reemplaza dias_vigencia por tipo_periodicidad
     ('Diario' | 'Semanal' | 'Mensual' | 'Anual').
   - Se agrega Facturacion (tabla intermedia entre Socio, Plan
     y Pago): representa un ciclo de cuota. Pago ahora cuelga
     de una Facturacion en vez de apuntar directo a Socio+Plan.
   ============================================================
   Las cinco tablas parametricas (Provincia, Localidad, Rol,
   TipoDocumento, MedioPago) llevan el campo 'activo' porque su
   baja es LOGICA (RF#04, RNF#03): el registro se marca inactivo
   y desaparece de grillas y combos, pero la fila queda para no
   invalidar los registros historicos que la referencian. Las
   consultas de la aplicacion filtran siempre por activo = 1.
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
    id_plan             INT IDENTITY(1,1) PRIMARY KEY,
    nombre              VARCHAR(100) NOT NULL,
    precio              DECIMAL(10,2) NOT NULL,
    tipo_periodicidad   VARCHAR(20) NOT NULL,
    activo              BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_Plan_TipoPeriodicidad
        CHECK (tipo_periodicidad IN ('Diario', 'Semanal', 'Mensual', 'Anual'))
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
    id_plan                   INT NULL,      -- plan preferido/actual, solo para precargar el combo al facturar
    fecha_vencimiento_cuota   DATE NULL,     -- cache de lectura rapida para Check-in (RNF#01); la fuente de verdad es Facturacion
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
   5. TESORERIA: Facturacion (depende de Socio y Plan) y
   Pago (depende de Facturacion y MedioPago)
   ============================================================ */

CREATE TABLE dbo.Facturacion (
    id_facturacion    INT IDENTITY(1,1) PRIMARY KEY,
    id_persona        INT NOT NULL,   -- Socio
    id_plan           INT NOT NULL,
    fecha_emision     DATE NOT NULL,
    fecha_vencimiento DATE NOT NULL,  -- fecha_emision + periodo segun Plan.tipo_periodicidad
    monto_total       DECIMAL(10,2) NOT NULL,  -- copia del precio del plan al momento de emitir (no se recalcula si el plan cambia de precio despues)
    estado            VARCHAR(20) NOT NULL DEFAULT 'Pendiente',
    CONSTRAINT FK_Facturacion_Socio
        FOREIGN KEY (id_persona) REFERENCES dbo.Socio(id_persona),
    CONSTRAINT FK_Facturacion_Plan
        FOREIGN KEY (id_plan) REFERENCES dbo.[Plan](id_plan),
    CONSTRAINT CK_Facturacion_Estado
        CHECK (estado IN ('Pendiente', 'Pagada', 'Vencida'))
);

CREATE TABLE dbo.Pago (
    id_pago         INT IDENTITY(1,1) PRIMARY KEY,
    id_facturacion  INT NOT NULL,
    id_medio_pago   INT NOT NULL,
    fecha_pago      DATE NOT NULL,
    monto           DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Pago_Facturacion
        FOREIGN KEY (id_facturacion) REFERENCES dbo.Facturacion(id_facturacion),
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
   7. ACTIVOS (Mantenimiento depende de Maquina y Usuario)
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

CREATE TABLE dbo.Mantenimiento (
    id_mantenimiento   INT IDENTITY(1,1) PRIMARY KEY,
    id_maquina         INT NOT NULL,
    id_persona         INT NOT NULL,   -- tecnico a cargo (usuario con rol Tecnico)
    fecha_inicio       DATE NOT NULL,
    fecha_fin          DATE NULL,
    detalle_tecnico    VARCHAR(500) NULL,
    CONSTRAINT FK_Mantenimiento_Maquina
        FOREIGN KEY (id_maquina) REFERENCES dbo.Maquina(id_maquina),
    CONSTRAINT FK_Mantenimiento_Usuario
        FOREIGN KEY (id_persona) REFERENCES dbo.Usuario(id_persona)
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

CREATE INDEX IX_Facturacion_Persona_Vencimiento
    ON dbo.Facturacion (id_persona, fecha_vencimiento DESC);

CREATE INDEX IX_Pago_Facturacion
    ON dbo.Pago (id_facturacion);

CREATE INDEX IX_Mantenimiento_Maquina
    ON dbo.Mantenimiento (id_maquina);

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

   contrasenia_hash es el SHA256 real de la contraseña "admin1234"
   (32 bytes), calculado con el mismo algoritmo que usa
   SGIG.Negocio.Hash.Calcular, para que este usuario pueda iniciar
   sesion apenas se crea la base. La contraseña nunca se guarda en
   texto plano (RNF#11): lo que viaja a la tabla es el hash.

   IMPORTANTE: cambiar esta contraseña desde frmUsuarios antes de
   entregar o poner el sistema en uso real.

   Para generar el hash de otra contraseña sin salir de la app:
       SGIG.Negocio.Hash.ATextoHex(SGIG.Negocio.Hash.Calcular("laQueSea"))
   devuelve el literal 0x... listo para pegar en este INSERT. */

INSERT INTO dbo.Persona (documento, id_tipo_documento, nombre, apellido, email)
VALUES ('00000001', 1, 'Admin', 'Sistema', 'admin@sgig.local');

INSERT INTO dbo.Usuario (id_persona, nombre_usuario, contrasenia_hash, id_rol, legajo, fecha_ingreso)
VALUES (
    SCOPE_IDENTITY(),
    'admin',
    0xAC9689E2272427085E35B9D3E3E8BED88CB3434828B43B86FC0596CAD4C6E270, -- SHA256 de "admin1234"
    (SELECT id_rol FROM dbo.Rol WHERE nombre_rol = 'Administrador'),
    'LEG-0001',
    GETDATE()
);

GO
