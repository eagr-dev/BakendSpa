-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 07-05-2026 a las 18:24:43
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.0.30

SET SESSION sql_require_primary_key = 0;
SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `spa_volta_vida`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `categorias`
--

CREATE TABLE `categorias` (
  `id_categoria` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `categorias`
--

INSERT INTO `categorias` (`id_categoria`, `nombre`) VALUES
(1, 'Terapias Tradicionales'),
(2, 'Tratamientos Faciales'),
(3, 'Depilaciones con Cera Española');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `citas`
--

CREATE TABLE `citas` (
  `id_cita` int(11) NOT NULL,
  `id_cliente` int(11) NOT NULL,
  `fecha` date NOT NULL,
  `hora_inicio` time NOT NULL,
  `hora_fin` time DEFAULT NULL,
  `estado` enum('pendiente','confirmada','cancelada') DEFAULT 'pendiente',
  `precio_total` decimal(10,2) NOT NULL,
  `anticipo` decimal(10,2) NOT NULL,
  `creado_en` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `citas`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `cita_servicios`
--

CREATE TABLE `cita_servicios` (
  `id_cita_servicio` int(11) NOT NULL,
  `id_cita` int(11) NOT NULL,
  `id_servicio` int(11) NOT NULL,
  `precio_unitario` decimal(10,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `cita_servicios`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `clientes`
--

CREATE TABLE `clientes` (
  `id_cliente` int(11) NOT NULL,
  `nombre` varchar(150) NOT NULL,
  `email` varchar(150) NOT NULL,
  `telefono` varchar(20) NOT NULL,
  `creado_en` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `clientes`
--


-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `notificaciones`
--

CREATE TABLE `notificaciones` (
  `id_notificacion` int(11) NOT NULL,
  `id_cita` int(11) NOT NULL,
  `destinatario` varchar(20) NOT NULL,
  `tipo` enum('cliente','duena') NOT NULL,
  `mensaje` text NOT NULL,
  `status` varchar(50) DEFAULT 'pendiente',
  `enviado_en` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `notificaciones`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `id_pago` int(11) NOT NULL,
  `id_cita` int(11) NOT NULL,
  `preference_id` varchar(255) DEFAULT NULL,
  `payment_id` varchar(255) DEFAULT NULL,
  `external_reference` varchar(255) DEFAULT NULL,
  `monto` decimal(10,2) NOT NULL,
  `status` varchar(50) DEFAULT 'pending',
  `collection_status` varchar(100) DEFAULT NULL,
  `payment_type` varchar(100) DEFAULT NULL,
  `processing_mode` varchar(100) DEFAULT NULL,
  `creado_en` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `pagos`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `servicios`
--

CREATE TABLE `servicios` (
  `id_servicio` int(11) NOT NULL,
  `id_categoria` int(11) NOT NULL,
  `nombre` varchar(150) NOT NULL,
  `descripcion` text DEFAULT NULL,
  `duracion_min` int(11) DEFAULT NULL,
  `precio` decimal(10,2) NOT NULL,
  `activo` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `servicios`
--

INSERT INTO `servicios` (`id_servicio`, `id_categoria`, `nombre`, `descripcion`, `duracion_min`, `precio`, `activo`) VALUES
(1, 1, 'Masaje Relajante 60 min', NULL, 60, 899.00, 1),
(2, 1, 'Masaje Relajante 90 min', NULL, 90, 1199.00, 1),
(3, 1, 'Masaje Relajante 120 min', NULL, 120, 1599.00, 1),
(4, 1, 'Masaje Drenaje Linfático 60 min', NULL, 60, 1099.00, 1),
(5, 1, 'Masaje Drenaje Linfático 90 min', NULL, 90, 1399.00, 1),
(6, 1, 'Masaje Drenaje Linfático 120 min', NULL, 120, 1799.00, 1),
(7, 1, 'Masaje Deportivo 60 min', NULL, 60, 1099.00, 1),
(8, 1, 'Masaje Deportivo 90 min', NULL, 90, 1399.00, 1),
(9, 1, 'Masaje Deportivo 120 min', NULL, 120, 1799.00, 1),
(10, 1, 'Masaje Descontracturante 60 min', NULL, 60, 1099.00, 1),
(11, 1, 'Masaje Descontracturante 90 min', NULL, 90, 1399.00, 1),
(12, 1, 'Masaje Descontracturante 120 min', NULL, 120, 1799.00, 1),
(13, 1, 'Masaje Prenatal 60 min', NULL, 60, 899.00, 1),
(14, 1, 'Masaje Prenatal 90 min', NULL, 90, 1199.00, 1),
(15, 1, 'Masaje Prenatal 120 min', NULL, 120, 1599.00, 1),
(16, 1, 'Veloterapia 60 min', NULL, 60, 1199.00, 1),
(17, 1, 'Masaje de Piedras Calientes 80 min', NULL, 80, 1399.00, 1),
(18, 1, 'Tratamiento Exfoliante Corporal 60 min', NULL, 60, 1299.00, 1),
(19, 2, 'Facial Reafirmante 50 min', NULL, 50, 400.00, 1),
(20, 3, 'Depilación Piernas Completas', NULL, NULL, 600.00, 1),
(21, 3, 'Depilación Medias Piernas', NULL, NULL, 400.00, 1),
(22, 3, 'Depilación Brazos', NULL, NULL, 300.00, 1),
(23, 3, 'Depilación Pecho', NULL, NULL, 400.00, 1),
(24, 3, 'Depilación Espalda', NULL, NULL, 400.00, 1),
(25, 3, 'Depilación Abdomen Completo', NULL, NULL, 400.00, 1),
(26, 3, 'Depilación Axilas', NULL, NULL, 250.00, 1),
(27, 3, 'Depilación Bikini', NULL, NULL, 400.00, 1),
(28, 3, 'Depilación Bikini Plus', NULL, NULL, 700.00, 1),
(29, 3, 'Depilación Glúteos', NULL, NULL, 450.00, 1),
(30, 3, 'Depilación Línea entre Glúteos', NULL, NULL, 400.00, 1),
(31, 3, 'Depilación Cejas', NULL, NULL, 250.00, 1);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `categorias`
--
ALTER TABLE `categorias`
  ADD PRIMARY KEY (`id_categoria`);

--
-- Indices de la tabla `citas`
--
ALTER TABLE `citas`
  ADD PRIMARY KEY (`id_cita`),
  ADD KEY `id_cliente` (`id_cliente`);

--
-- Indices de la tabla `cita_servicios`
--
ALTER TABLE `cita_servicios`
  ADD PRIMARY KEY (`id_cita_servicio`),
  ADD KEY `id_cita` (`id_cita`),
  ADD KEY `id_servicio` (`id_servicio`);

--
-- Indices de la tabla `clientes`
--
ALTER TABLE `clientes`
  ADD PRIMARY KEY (`id_cliente`),
  ADD UNIQUE KEY `email` (`email`);

--
-- Indices de la tabla `notificaciones`
--
ALTER TABLE `notificaciones`
  ADD PRIMARY KEY (`id_notificacion`),
  ADD KEY `id_cita` (`id_cita`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`id_pago`),
  ADD KEY `id_cita` (`id_cita`);

--
-- Indices de la tabla `servicios`
--
ALTER TABLE `servicios`
  ADD PRIMARY KEY (`id_servicio`),
  ADD KEY `id_categoria` (`id_categoria`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `categorias`
--
ALTER TABLE `categorias`
  MODIFY `id_categoria` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `citas`
--
ALTER TABLE `citas`
  MODIFY `id_cita` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT de la tabla `cita_servicios`
--
ALTER TABLE `cita_servicios`
  MODIFY `id_cita_servicio` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=28;

--
-- AUTO_INCREMENT de la tabla `clientes`
--
ALTER TABLE `clientes`
  MODIFY `id_cliente` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `notificaciones`
--
ALTER TABLE `notificaciones`
  MODIFY `id_notificacion` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=155;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `id_pago` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT de la tabla `servicios`
--
ALTER TABLE `servicios`
  MODIFY `id_servicio` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=32;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `citas`
--
ALTER TABLE `citas`
  ADD CONSTRAINT `citas_ibfk_1` FOREIGN KEY (`id_cliente`) REFERENCES `clientes` (`id_cliente`);

--
-- Filtros para la tabla `cita_servicios`
--
ALTER TABLE `cita_servicios`
  ADD CONSTRAINT `cita_servicios_ibfk_1` FOREIGN KEY (`id_cita`) REFERENCES `citas` (`id_cita`),
  ADD CONSTRAINT `cita_servicios_ibfk_2` FOREIGN KEY (`id_servicio`) REFERENCES `servicios` (`id_servicio`);

--
-- Filtros para la tabla `notificaciones`
--
ALTER TABLE `notificaciones`
  ADD CONSTRAINT `notificaciones_ibfk_1` FOREIGN KEY (`id_cita`) REFERENCES `citas` (`id_cita`);

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `pagos_ibfk_1` FOREIGN KEY (`id_cita`) REFERENCES `citas` (`id_cita`);

--
-- Filtros para la tabla `servicios`
--
ALTER TABLE `servicios`
  ADD CONSTRAINT `servicios_ibfk_1` FOREIGN KEY (`id_categoria`) REFERENCES `categorias` (`id_categoria`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
