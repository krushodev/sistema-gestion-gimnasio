/* ============================================================
   Sistema de Gestion Integral para Gimnasios (SGIG)
   Seed HISTORICO (por defecto 5 anios) para demo y simulacion
   ============================================================
   Requisito: haber corrido antes SGIG_CreateDB.sql (esquema + catalogos).

   Todo es RELATIVO A HOY: la ventana siempre termina en la fecha en que
   se corre el script. Para cambiar el largo de la historia, editar @anios
   (1 a 6; con mas de 6 hay que ampliar #N).

   Que genera:
     - Personal (2 Recepcionistas y 2 Tecnicos) si todavia no existe.
     - 450 Socios (documentos 60000001..60000450) con una base inicial de
       60 "veteranos" y altas que crecen con el tiempo y se concentran en
       ene-mar y ago-sep. Mix de planes: ~8 % diario, ~10 % semanal,
       ~65 % mensual, ~17 % anual. Bajas, morosos, cuotas pendientes y
       socios que dejan de venir.
     - Facturaciones por ciclo de cuota (monto deflactado ~2 % mensual
       hacia atras, asi se ve la inflacion en los graficos) y sus Pagos.
       El medio de pago cambia con los anios: baja el efectivo, suben
       tarjeta y transferencia. Algunos pagos se dividen en dos.
     - Check-ins (Concedido/Rechazado) con estacionalidad mensual (verano y
       vacaciones de invierno flojos, pico post-ano nuevo), dia de la
       semana y picos de horario (manana temprano y 18-21 h). Los
       rechazados son socios que intentan entrar recien vencida la cuota.
     - ~350 Mantenimientos historicos + 4 abiertos (maquina En Reparacion).

   Es RE-EJECUTABLE: al empezar borra lo que genero antes (socios 6000xxxx
   con sus facturas/pagos/check-ins, y los mantenimientos de los tecnicos
   lferreyra/dparedes). No toca socios ni usuarios cargados a mano.
   Es DETERMINISTA salvo por la fecha de hoy: dos corridas el mismo dia
   producen los mismos datos.
   Contrasenia del personal sembrado: Gimnasio2026!
   ============================================================ */

USE SGIG;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @anios   INT  = 5;                                   -- <<< largo de la historia
DECLARE @nSocios INT  = 450;
DECLARE @nVeter  INT  = 60;                                  -- socios que ya estaban al arrancar
DECLARE @hoy     DATE = CAST(GETDATE() AS DATE);
DECLARE @desde   DATE = DATEADD(DAY, 1, DATEADD(YEAR, -@anios, @hoy));
DECLARE @dias    INT  = DATEDIFF(DAY, @desde, @hoy) + 1;
DECLARE @hashPersonal VARBINARY(256) =
    0xCDC1A94BF99D376CE5C13715E5FC4CE3CD6E8248CB4B397B20A92721CC648C48; -- SHA256 de "Gimnasio2026!"

BEGIN TRAN;

/* ---------- 0. Limpieza de una corrida anterior ---------- */
DELETE c  FROM dbo.Checkin c
    JOIN dbo.Persona p ON p.id_persona = c.id_persona
    WHERE p.documento BETWEEN '60000001' AND '60009999';
DELETE pg FROM dbo.Pago pg
    JOIN dbo.Facturacion f ON f.id_facturacion = pg.id_facturacion
    JOIN dbo.Persona p ON p.id_persona = f.id_persona
    WHERE p.documento BETWEEN '60000001' AND '60009999';
DELETE f  FROM dbo.Facturacion f
    JOIN dbo.Persona p ON p.id_persona = f.id_persona
    WHERE p.documento BETWEEN '60000001' AND '60009999';
DELETE FROM dbo.Mantenimiento
    WHERE id_persona IN (SELECT id_persona FROM dbo.Usuario WHERE nombre_usuario IN ('lferreyra', 'dparedes'));
DELETE s  FROM dbo.Socio s
    JOIN dbo.Persona p ON p.id_persona = s.id_persona
    WHERE p.documento BETWEEN '60000001' AND '60009999';
DELETE FROM dbo.Persona WHERE documento BETWEEN '60000001' AND '60009999';

/* ---------- 1. Personal (si falta) ---------- */
DECLARE @dni INT = (SELECT TOP 1 id_tipo_documento FROM dbo.TipoDocumento WHERE descripcion = 'DNI');
DECLARE @idp INT;

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE nombre_usuario = 'aibarra')
   AND NOT EXISTS (SELECT 1 FROM dbo.Persona WHERE documento = '33111222')
BEGIN
    INSERT dbo.Persona (documento, id_tipo_documento, nombre, apellido, email, telefono)
    VALUES ('33111222', @dni, 'Ana', 'Ibarra', 'ana.ibarra@sgig.local', '3794112233');
    SET @idp = SCOPE_IDENTITY();
    INSERT dbo.Usuario (id_persona, nombre_usuario, contrasenia_hash, id_rol, legajo, fecha_ingreso)
    VALUES (@idp, 'aibarra', @hashPersonal, (SELECT id_rol FROM dbo.Rol WHERE nombre_rol = 'Recepcionista'), 'LEG-0002', '2024-03-01');
END
IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE nombre_usuario = 'cmedina')
   AND NOT EXISTS (SELECT 1 FROM dbo.Persona WHERE documento = '33222333')
BEGIN
    INSERT dbo.Persona (documento, id_tipo_documento, nombre, apellido, email, telefono)
    VALUES ('33222333', @dni, 'Carlos', 'Medina', 'carlos.medina@sgig.local', '3794223344');
    SET @idp = SCOPE_IDENTITY();
    INSERT dbo.Usuario (id_persona, nombre_usuario, contrasenia_hash, id_rol, legajo, fecha_ingreso)
    VALUES (@idp, 'cmedina', @hashPersonal, (SELECT id_rol FROM dbo.Rol WHERE nombre_rol = 'Recepcionista'), 'LEG-0003', '2024-06-15');
END
IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE nombre_usuario = 'lferreyra')
   AND NOT EXISTS (SELECT 1 FROM dbo.Persona WHERE documento = '33333444')
BEGIN
    INSERT dbo.Persona (documento, id_tipo_documento, nombre, apellido, email, telefono)
    VALUES ('33333444', @dni, 'Lucia', 'Ferreyra', 'lucia.ferreyra@sgig.local', '3794334455');
    SET @idp = SCOPE_IDENTITY();
    INSERT dbo.Usuario (id_persona, nombre_usuario, contrasenia_hash, id_rol, legajo, fecha_ingreso)
    VALUES (@idp, 'lferreyra', @hashPersonal, (SELECT id_rol FROM dbo.Rol WHERE nombre_rol = 'Tecnico'), 'LEG-0004', '2023-11-10');
END
IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE nombre_usuario = 'dparedes')
   AND NOT EXISTS (SELECT 1 FROM dbo.Persona WHERE documento = '33444555')
BEGIN
    INSERT dbo.Persona (documento, id_tipo_documento, nombre, apellido, email, telefono)
    VALUES ('33444555', @dni, 'Diego', 'Paredes', 'diego.paredes@sgig.local', '3794445566');
    SET @idp = SCOPE_IDENTITY();
    INSERT dbo.Usuario (id_persona, nombre_usuario, contrasenia_hash, id_rol, legajo, fecha_ingreso)
    VALUES (@idp, 'dparedes', @hashPersonal, (SELECT id_rol FROM dbo.Rol WHERE nombre_rol = 'Tecnico'), 'LEG-0005', '2025-02-20');
END

/* ---------- 2. Tablas auxiliares ---------- */
SELECT TOP (2300) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n
INTO #N FROM sys.all_objects a CROSS JOIN sys.all_objects b;

SELECT v.rn, v.txt INTO #Nombre FROM (VALUES
 (1,'Martina'),(2,'Lucas'),(3,'Sofia'),(4,'Mateo'),(5,'Valentina'),(6,'Benjamin'),(7,'Camila'),(8,'Joaquin'),
 (9,'Isabella'),(10,'Thiago'),(11,'Julieta'),(12,'Santiago'),(13,'Agustina'),(14,'Nicolas'),(15,'Florencia'),
 (16,'Facundo'),(17,'Micaela'),(18,'Franco'),(19,'Rocio'),(20,'Tomas'),(21,'Candela'),(22,'Bautista'),
 (23,'Lucia'),(24,'Emiliano'),(25,'Abril'),(26,'Gonzalo'),(27,'Paula'),(28,'Ignacio'),(29,'Carolina'),
 (30,'Matias'),(31,'Brenda'),(32,'Ramiro'),(33,'Daniela'),(34,'Lautaro'),(35,'Milagros'),(36,'Ezequiel'),
 (37,'Antonella'),(38,'Federico'),(39,'Romina'),(40,'Maximiliano'),(41,'Pilar'),(42,'Dante'),
 (43,'Renata'),(44,'Alan'),(45,'Julian'),(46,'Noelia'),(47,'Sebastian'),(48,'Gabriela'),
 (49,'Leandro'),(50,'Marina')) v(rn, txt);

SELECT v.rn, v.txt INTO #Apellido FROM (VALUES
 (1,'Gomez'),(2,'Fernandez'),(3,'Rodriguez'),(4,'Lopez'),(5,'Diaz'),(6,'Torres'),(7,'Sosa'),(8,'Romero'),
 (9,'Flores'),(10,'Acosta'),(11,'Benitez'),(12,'Medina'),(13,'Herrera'),(14,'Suarez'),(15,'Aguirre'),
 (16,'Gimenez'),(17,'Gutierrez'),(18,'Pereyra'),(19,'Rojas'),(20,'Molina'),(21,'Castro'),(22,'Ortiz'),
 (23,'Silva'),(24,'Nunez'),(25,'Luna'),(26,'Juarez'),(27,'Cabrera'),(28,'Rios'),(29,'Vega'),(30,'Duarte'),
 (31,'Barrios'),(32,'Ledesma'),(33,'Maidana'),(34,'Veron'),(35,'Cardozo'),(36,'Ojeda'),
 (37,'Sandoval'),(38,'Quiroga'),(39,'Peralta'),(40,'Villalba')) v(rn, txt);

SELECT ROW_NUMBER() OVER (ORDER BY id_localidad) AS rn, id_localidad AS id INTO #Loc FROM dbo.Localidad WHERE activo = 1;
DECLARE @nLoc INT = (SELECT COUNT(*) FROM #Loc);
DECLARE @locCtes INT = (SELECT TOP 1 l.id_localidad FROM dbo.Localidad l JOIN dbo.Provincia p ON p.id_provincia = l.id_provincia
                        WHERE l.nombre = 'Corrientes' AND p.nombre = 'Corrientes');
DECLARE @locResi INT = (SELECT TOP 1 l.id_localidad FROM dbo.Localidad l JOIN dbo.Provincia p ON p.id_provincia = l.id_provincia
                        WHERE l.nombre = 'Resistencia' AND p.nombre = 'Chaco');

/* Horas de ingreso con sus pesos (cada fila repetida = mas probabilidad) */
SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn, h INTO #Hora FROM (VALUES
 (6),(7),(7),(7),(8),(8),(8),(9),(9),(10),(11),(12),(13),(14),(15),(16),(16),(17),(17),(17),
 (18),(18),(18),(18),(19),(19),(19),(19),(20),(20),(20),(21)) v(h);

/* Dias de la ventana: peso para repartir las altas (estacionalidad x crecimiento) */
SELECT n, DATEADD(DAY, n, @desde) AS dia,
       (CASE WHEN MONTH(DATEADD(DAY, n, @desde)) IN (1,2,3) THEN 3
             WHEN MONTH(DATEADD(DAY, n, @desde)) IN (8,9)   THEN 2 ELSE 1 END)
       * (4 + n * 8 / @dias) AS peso
INTO #Dias FROM #N WHERE n < @dias;
ALTER TABLE #Dias ADD acum INT NULL;
UPDATE d SET acum = x.a FROM #Dias d
    JOIN (SELECT n, SUM(peso) OVER (ORDER BY n ROWS UNBOUNDED PRECEDING) AS a FROM #Dias) x ON x.n = d.n;
DECLARE @pesoTotal INT = (SELECT MAX(acum) FROM #Dias);

/* ---------- 3. Socios (definicion en memoria) ---------- */
SELECT i,
       CASE WHEN h13 < 8 THEN 1 WHEN h13 < 18 THEN 2 WHEN h13 < 83 THEN 3 ELSE 4 END AS plan_n,
       CAST(NULL AS DATE) AS inicio,
       CAST(NULL AS INT)  AS ncic,       -- cantidad de ciclos si se da de baja (NULL = sigue)
       h14 AS pct_baja, h15 AS pct_moroso, h16 AS visitas_sem,
       CAST(NULL AS INT)  AS id_persona
INTO #S
FROM (SELECT t.n AS i,
             ABS(CHECKSUM(t.n, 13)) % 100 AS h13, ABS(CHECKSUM(t.n, 14)) % 100 AS h14,
             ABS(CHECKSUM(t.n, 15)) % 100 AS h15, 1 + ABS(CHECKSUM(t.n, 16)) % 5 AS h16
      FROM #N t WHERE t.n BETWEEN 1 AND @nSocios) q;

UPDATE s SET inicio =
    CASE WHEN s.i <= @nVeter THEN DATEADD(DAY, ABS(CHECKSUM(s.i, 12)) % 15, @desde)
         ELSE (SELECT d.dia FROM #Dias d
               WHERE ABS(CHECKSUM(s.i, 11)) % @pesoTotal >= d.acum - d.peso
                 AND ABS(CHECKSUM(s.i, 11)) % @pesoTotal <  d.acum) END
FROM #S s;

/* Bajas: ~45 % de los socios deja de renovar tras una cantidad de ciclos que depende del plan */
UPDATE s SET ncic = CASE WHEN s.pct_baja < 45 THEN
        CASE s.plan_n WHEN 1 THEN 2 + ABS(CHECKSUM(s.i, 21)) % 12
                      WHEN 2 THEN 3 + ABS(CHECKSUM(s.i, 21)) % 30
                      WHEN 3 THEN 2 + ABS(CHECKSUM(s.i, 21)) % 30
                      ELSE 1 + ABS(CHECKSUM(s.i, 21)) % 3 END
        ELSE NULL END
FROM #S s;

/* ---------- 4. Persona + Socio ---------- */
INSERT dbo.Persona (documento, id_tipo_documento, nombre, apellido, email, telefono, id_localidad, fecha_nacimiento)
SELECT CAST(60000000 + s.i AS VARCHAR(8)), @dni, nm.txt, ap.txt,
       LOWER(nm.txt + '.' + ap.txt + CAST(s.i AS VARCHAR(4)) + '@gmail.com'),
       '3794' + RIGHT('000000' + CAST(100000 + s.i * 3571 % 900000 AS VARCHAR(6)), 6),
       CASE WHEN ABS(CHECKSUM(s.i, 31)) % 100 < 60 THEN @locCtes
            WHEN ABS(CHECKSUM(s.i, 31)) % 100 < 85 THEN @locResi
            ELSE (SELECT id FROM #Loc WHERE rn = ABS(CHECKSUM(s.i, 32)) % @nLoc + 1) END,
       DATEADD(DAY, ABS(CHECKSUM(s.i, 33)) % 16000, '1965-01-01')
FROM #S s
JOIN #Nombre nm ON nm.rn = ABS(CHECKSUM(s.i, 41)) % 50 + 1
JOIN #Apellido ap ON ap.rn = ABS(CHECKSUM(s.i, 42)) % 40 + 1;

UPDATE s SET id_persona = p.id_persona
FROM #S s JOIN dbo.Persona p ON p.documento = CAST(60000000 + s.i AS VARCHAR(8));

/* ---------- 5. Ciclos de cuota (Facturacion) en memoria ---------- */
SELECT s.i, k.n AS k, s.plan_n, c.emision, c.venc,
       CAST(1 AS BIT) AS pagada, CAST(0 AS BIT) AS dividido, CAST(NULL AS DECIMAL(10,2)) AS monto
INTO #F
FROM #S s
JOIN #N k ON k.n < 300 AND k.n < ISNULL(s.ncic, 1000)
CROSS APPLY (SELECT
      CASE s.plan_n WHEN 1 THEN DATEADD(DAY, 9 * k.n, s.inicio)
                    WHEN 2 THEN DATEADD(WEEK, k.n, s.inicio)
                    WHEN 3 THEN DATEADD(MONTH, k.n, s.inicio)
                    ELSE DATEADD(YEAR, k.n, s.inicio) END AS emision,
      CASE s.plan_n WHEN 1 THEN DATEADD(DAY, 9 * k.n + 1, s.inicio)
                    WHEN 2 THEN DATEADD(WEEK, k.n + 1, s.inicio)
                    WHEN 3 THEN DATEADD(MONTH, k.n + 1, s.inicio)
                    ELSE DATEADD(YEAR, k.n + 1, s.inicio) END AS venc) c
WHERE c.emision <= @hoy;
CREATE CLUSTERED INDEX IX_F ON #F (i, emision);

/* Morosos: el ultimo ciclo queda sin pagar (solo semanal/mensual con >= 2 ciclos) */
UPDATE f SET pagada = 0
FROM #F f
JOIN #S s ON s.i = f.i AND s.pct_moroso < 7 AND s.plan_n IN (2, 3)
WHERE f.k = (SELECT MAX(k) FROM #F x WHERE x.i = f.i) AND f.k >= 1;

/* Cuotas recien emitidas y todavia sin pagar (Pendiente) */
UPDATE f SET pagada = 0
FROM #F f
JOIN #S s ON s.i = f.i AND s.plan_n IN (3, 4)
WHERE f.k = (SELECT MAX(k) FROM #F x WHERE x.i = f.i)
  AND f.emision >= DATEADD(DAY, -3, @hoy)
  AND ABS(CHECKSUM(f.i, 52)) % 100 < 20;

/* Monto vigente a la fecha de emision: precio actual deflactado ~2 % por mes hacia atras */
UPDATE f SET monto = ROUND(pl.precio * POWER(CAST(0.98 AS FLOAT), DATEDIFF(MONTH, f.emision, @hoy)) / 100.0, 0) * 100.0,
             dividido = CASE WHEN pl.precio >= 12000 AND ABS(CHECKSUM(f.i, f.k, 51)) % 25 = 0 THEN 1 ELSE 0 END
FROM #F f
JOIN dbo.[Plan] pl ON pl.tipo_periodicidad = CASE f.plan_n WHEN 1 THEN 'Diario' WHEN 2 THEN 'Semanal'
                                                          WHEN 3 THEN 'Mensual' ELSE 'Anual' END;

/* Socio: plan preferido, cache de vencimiento y baja logica */
INSERT dbo.Socio (id_persona, apto_medico, id_plan, fecha_vencimiento_cuota, activo)
SELECT s.id_persona,
       CASE WHEN ABS(CHECKSUM(s.i, 61)) % 100 < 75 THEN 'Apto vigente' ELSE NULL END,
       (SELECT TOP 1 id_plan FROM dbo.[Plan] WHERE tipo_periodicidad =
            CASE s.plan_n WHEN 1 THEN 'Diario' WHEN 2 THEN 'Semanal' WHEN 3 THEN 'Mensual' ELSE 'Anual' END),
       (SELECT MAX(f.venc) FROM #F f WHERE f.i = s.i AND f.pagada = 1),
       CASE WHEN s.ncic IS NOT NULL AND s.pct_baja < 36
                 AND (SELECT MAX(f.venc) FROM #F f WHERE f.i = s.i) < @hoy THEN 0 ELSE 1 END
FROM #S s;

/* ---------- 6. Facturacion y Pago ---------- */
INSERT dbo.Facturacion (id_persona, id_plan, fecha_emision, fecha_vencimiento, monto_total, estado)
SELECT s.id_persona,
       (SELECT TOP 1 id_plan FROM dbo.[Plan] WHERE tipo_periodicidad =
            CASE f.plan_n WHEN 1 THEN 'Diario' WHEN 2 THEN 'Semanal' WHEN 3 THEN 'Mensual' ELSE 'Anual' END),
       f.emision, f.venc, f.monto,
       CASE WHEN f.pagada = 1 THEN 'Pagada' WHEN f.venc < @hoy THEN 'Vencida' ELSE 'Pendiente' END
FROM #F f JOIN #S s ON s.i = f.i;

DECLARE @mEfec INT = (SELECT TOP 1 id_medio_pago FROM dbo.MedioPago WHERE descripcion = 'Efectivo');
DECLARE @mTarj INT = (SELECT TOP 1 id_medio_pago FROM dbo.MedioPago WHERE descripcion = 'Tarjeta');
DECLARE @mTran INT = (SELECT TOP 1 id_medio_pago FROM dbo.MedioPago WHERE descripcion = 'Transferencia');

/* Pago principal (o la primera mitad si esta dividido).
   El medio evoluciona: transferencia 10 % -> 40 %, tarjeta 25 % -> 40 %, el resto efectivo. */
INSERT dbo.Pago (id_facturacion, id_medio_pago, fecha_pago, monto)
SELECT fa.id_facturacion,
       CASE WHEN u.v < 10 + prog.p * 30 / 100 THEN @mTran
            WHEN u.v < 10 + prog.p * 30 / 100 + 25 + prog.p * 15 / 100 THEN @mTarj
            ELSE @mEfec END,
       CASE WHEN DATEADD(DAY, ABS(CHECKSUM(f.i, f.k, 72)) % 3, f.emision) > @hoy THEN @hoy
            ELSE DATEADD(DAY, ABS(CHECKSUM(f.i, f.k, 72)) % 3, f.emision) END,
       CASE WHEN f.dividido = 1 THEN ROUND(f.monto / 2, 0) ELSE f.monto END
FROM #F f
JOIN #S s ON s.i = f.i
JOIN dbo.Facturacion fa ON fa.id_persona = s.id_persona AND fa.fecha_emision = f.emision
CROSS APPLY (SELECT 100 * DATEDIFF(DAY, @desde, f.emision) / NULLIF(@dias, 0) AS p) prog
CROSS APPLY (SELECT ABS(CHECKSUM(f.i, f.k, 71)) % 100 AS v) u
WHERE f.pagada = 1;

/* Segunda mitad de los pagos divididos (siempre en efectivo) */
INSERT dbo.Pago (id_facturacion, id_medio_pago, fecha_pago, monto)
SELECT fa.id_facturacion, @mEfec,
       CASE WHEN DATEADD(DAY, 3, f.emision) > @hoy THEN @hoy ELSE DATEADD(DAY, 3, f.emision) END,
       f.monto - ROUND(f.monto / 2, 0)
FROM #F f
JOIN #S s ON s.i = f.i
JOIN dbo.Facturacion fa ON fa.id_persona = s.id_persona AND fa.fecha_emision = f.emision
WHERE f.pagada = 1 AND f.dividido = 1;

/* ---------- 7. Check-ins ---------- */
/* Un socio puede venir (a) dentro de una cuota paga, o (b) hasta 21 dias despues de que
   vencio (intento rechazado). Fuera de eso no aparece. */
INSERT dbo.Checkin (id_persona, fecha_hora, resultado)
SELECT s.id_persona,
       DATEADD(MINUTE, h.h * 60 + ABS(CHECKSUM(s.i, d.n, 83)) % 60, CAST(d.dia AS DATETIME)),
       CASE WHEN est.cubierto = 1 THEN 'Concedido' ELSE 'Rechazado' END
FROM #S s
JOIN #Dias d ON d.dia >= s.inicio
            AND d.dia <= CASE WHEN s.ncic IS NULL THEN @hoy
                              ELSE (SELECT MIN(x.f) FROM (VALUES (@hoy),
                                      (DATEADD(DAY, 21, (SELECT MAX(venc) FROM #F WHERE i = s.i)))) x(f)) END
CROSS APPLY (SELECT
      CASE WHEN EXISTS (SELECT 1 FROM #F f WHERE f.i = s.i AND f.pagada = 1
                         AND d.dia >= f.emision AND d.dia <= f.venc) THEN 1 ELSE 0 END AS cubierto,
      CASE WHEN EXISTS (SELECT 1 FROM #F f WHERE f.i = s.i AND f.pagada = 1
                         AND d.dia > f.venc AND d.dia <= DATEADD(DAY, 21, f.venc)) THEN 1 ELSE 0 END AS reciente) est
CROSS APPLY (SELECT (ABS(CHECKSUM(s.i, d.n, 81)) % 1000) / 1000.0 AS u,
                    s.visitas_sem / 7.0
                    * CASE (DATEDIFF(DAY, '19000101', d.dia) % 7)          -- 0 = lunes
                           WHEN 5 THEN 0.7 WHEN 6 THEN 0.2 ELSE 1.15 END
                    * CASE MONTH(d.dia) WHEN 1 THEN 0.85 WHEN 2 THEN 1.1 WHEN 3 THEN 1.2 WHEN 4 THEN 1.05
                                        WHEN 5 THEN 1.0 WHEN 6 THEN 0.95 WHEN 7 THEN 0.8 WHEN 8 THEN 1.0
                                        WHEN 9 THEN 1.1 WHEN 10 THEN 1.05 WHEN 11 THEN 1.0 ELSE 0.8 END AS p) r
JOIN #Hora h ON h.rn = ABS(CHECKSUM(s.i, d.n, 82)) % 32 + 1
WHERE ((est.cubierto = 1 AND r.u < r.p)
    OR (est.cubierto = 0 AND est.reciente = 1 AND s.plan_n <> 1 AND r.u < r.p * 0.35))
  AND DATEADD(MINUTE, h.h * 60, CAST(d.dia AS DATETIME)) <= GETDATE();

/* ---------- 8. Mantenimientos ---------- */
SELECT ROW_NUMBER() OVER (ORDER BY id_maquina) AS rn, id_maquina AS id, fecha_compra INTO #Maq FROM dbo.Maquina;
DECLARE @nMaq INT = (SELECT COUNT(*) FROM #Maq);
DECLARE @tec1 INT = (SELECT id_persona FROM dbo.Usuario WHERE nombre_usuario = 'lferreyra');
DECLARE @tec2 INT = (SELECT id_persona FROM dbo.Usuario WHERE nombre_usuario = 'dparedes');

SELECT v.rn, v.txt INTO #Det FROM (VALUES
 (1,'Lubricacion de guias y revision de cables'),(2,'Cambio de correa de transmision'),
 (3,'Ajuste de tension y calibracion del sensor'),(4,'Reemplazo de rodamientos'),
 (5,'Reparacion del tablero de control'),(6,'Cambio de tapizado y almohadillas'),
 (7,'Revision electrica general y limpieza de contactos'),(8,'Reemplazo de cable de acero'),
 (9,'Nivelacion de base y ajuste de tornilleria'),(10,'Mantenimiento preventivo semestral'),
 (11,'Reemplazo de pedales y calapies'),(12,'Cambio de motor de la cinta'),
 (13,'Ajuste de la banda de rodamiento'),(14,'Reparacion de polea y guia de carga'),
 (15,'Actualizacion del software de la consola'),(16,'Cambio de manijas y topes de goma')) v(rn, txt);

/* Historicos cerrados: unos 6 por mes, repartidos en toda la ventana */
INSERT dbo.Mantenimiento (id_maquina, id_persona, fecha_inicio, fecha_fin, detalle_tecnico)
SELECT m.id,
       CASE WHEN q.n % 2 = 0 THEN @tec1 ELSE @tec2 END,
       q.ini, DATEADD(DAY, q.dur, q.ini),
       dt.txt
FROM (SELECT n,
             ABS(CHECKSUM(n, 91)) % @nMaq + 1 AS mrn,
             DATEADD(DAY, ABS(CHECKSUM(n, 92)) % (@dias - 15), @desde) AS ini,
             1 + ABS(CHECKSUM(n, 93)) % 14 AS dur,
             ABS(CHECKSUM(n, 94)) % 16 + 1 AS drn
      FROM #N WHERE n < 6 * 12 * @anios) q
JOIN #Maq m ON m.rn = q.mrn
JOIN #Det dt ON dt.rn = q.drn
WHERE DATEADD(DAY, q.dur, q.ini) <= @hoy
  AND (m.fecha_compra IS NULL OR q.ini >= m.fecha_compra);

/* Abiertos: 4 maquinas hoy en reparacion */
UPDATE dbo.Maquina SET estado = 'Operativa';
INSERT dbo.Mantenimiento (id_maquina, id_persona, fecha_inicio, fecha_fin, detalle_tecnico)
SELECT m.id, CASE WHEN o.n % 2 = 0 THEN @tec1 ELSE @tec2 END,
       DATEADD(DAY, -o.n, @hoy), NULL, dt.txt
FROM (VALUES (1, 3), (2, 2), (3, 1), (4, 4)) o(n, mrn_off)
JOIN #Maq m ON m.rn = o.mrn_off * 5
JOIN #Det dt ON dt.rn = o.n + 4;
UPDATE mq SET estado = 'En Reparacion'
FROM dbo.Maquina mq
WHERE EXISTS (SELECT 1 FROM dbo.Mantenimiento mt WHERE mt.id_maquina = mq.id_maquina AND mt.fecha_fin IS NULL);

COMMIT TRAN;

/* ---------- Resumen ---------- */
SELECT (SELECT COUNT(*) FROM dbo.Usuario)                         AS Usuarios,
       (SELECT COUNT(*) FROM dbo.Socio)                           AS Socios,
       (SELECT COUNT(*) FROM dbo.Socio WHERE activo = 1)          AS SociosActivos,
       (SELECT COUNT(*) FROM dbo.Facturacion)                     AS Facturaciones,
       (SELECT COUNT(*) FROM dbo.Pago)                            AS Pagos,
       (SELECT SUM(monto) FROM dbo.Pago)                          AS TotalCobrado,
       (SELECT COUNT(*) FROM dbo.Checkin)                         AS Checkins,
       (SELECT COUNT(*) FROM dbo.Mantenimiento)                   AS Mantenimientos,
       (SELECT COUNT(*) FROM dbo.Maquina WHERE estado = 'En Reparacion') AS EnReparacion;

SELECT YEAR(fecha_pago) AS Anio, COUNT(*) AS Pagos, SUM(monto) AS Cobrado
FROM dbo.Pago GROUP BY YEAR(fecha_pago) ORDER BY 1;
GO
