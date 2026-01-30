-- 1. TABLAS MAESTRAS
SET default_storage_engine=InnoDB;

CREATE TABLE Proveedor (
    IdProveedor INT NOT NULL AUTO_INCREMENT,
 NombreEmpresa VARCHAR(100) NOT NULL,
 Direccion VARCHAR(255) NOT NULL,
 Telefono VARCHAR(15) NOT NULL,
 Email VARCHAR(100) NOT NULL,
 Estado ENUM('Activo', 'Inactivo') NOT NULL,
 PRIMARY KEY (IdProveedor),
 UNIQUE INDEX idx_nombre_empresa_unico (NombreEmpresa)
);

CREATE TABLE Producto (
    IdProducto INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL UNIQUE,
    Descripcion TEXT,
    CantidadStock INT NOT NULL DEFAULT 0,
    UnidadMedida VARCHAR(20) NOT NULL,
    Precio DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    Estado ENUM('Disponible', 'Agotado') NOT NULL DEFAULT 'Agotado',
    CHECK (CantidadStock >= 0)
);

CREATE TABLE Usuario (
   IdUsuario INT NOT NULL AUTO_INCREMENT,
Nombre VARCHAR(100) NOT NULL,
Email VARCHAR(100) NOT NULL UNIQUE,
TipoRol ENUM('Administrador') NOT NULL,
Contraseña VARCHAR(255) NOT NULL,
Estado ENUM('Activo', 'Inactivo') NOT NULL,
PRIMARY KEY (IdUsuario)
);

CREATE TABLE Departamento (
    IdDepartamento INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL UNIQUE,
    Descripcion TEXT
);

CREATE TABLE Empleado (
    IdEmpleado INT NOT NULL AUTO_INCREMENT,
 Nombre VARCHAR(100) NOT NULL,
 Apellido VARCHAR(100) NOT NULL,
 Telefono VARCHAR(15) NOT NULL,
 Edad TINYINT UNSIGNED NOT NULL,
 Genero ENUM('Masculino', 'Femenino') NOT NULL,
 Direccion VARCHAR(255) NOT NULL,
 DepartamentoId INT NOT NULL,
 Estado ENUM('Activo', 'Inactivo') NOT NULL,
 PRIMARY KEY (IdEmpleado),
 FOREIGN KEY (DepartamentoId) REFERENCES Departamento(IdDepartamento) ON DELETE RESTRICT ON UPDATE CASCADE,
 CHECK (Edad > 0)
);

-- 2. CABECERAS (MAESTRO)
CREATE TABLE Compra (
    IdCompra INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    NumeroFactura VARCHAR(50) NOT NULL,
    UsuarioId INT NOT NULL,
    ProveedorId INT NOT NULL,
    FechaCompra DATE NOT NULL,
    MontoTotalCompra DECIMAL(10, 2) DEFAULT 0.00,
    FOREIGN KEY (UsuarioId) REFERENCES Usuario(IdUsuario),
    FOREIGN KEY (ProveedorId) REFERENCES Proveedor(IdProveedor)
);

CREATE TABLE Distribucion (
    IdDistribucion INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    NumeroDistribucion VARCHAR(50) NOT NULL,
    UsuarioId INT NOT NULL,
    EmpleadoId INT NOT NULL,
    FechaSalida DATE NOT NULL,
    Motivo VARCHAR(255),
    MontoTotalDistribucion DECIMAL(10, 2) DEFAULT 0.00,
    FOREIGN KEY (UsuarioId) REFERENCES Usuario(IdUsuario),
    FOREIGN KEY (EmpleadoId) REFERENCES Empleado(IdEmpleado)
);

-- 3. DETALLES
CREATE TABLE DetalleCompra (
    IdDetalleCompra INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    CompraId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitarioCosto DECIMAL(10, 2) NOT NULL,
    Subtotal DECIMAL(10, 2) AS (Cantidad * PrecioUnitarioCosto) STORED,
    FOREIGN KEY (CompraId) REFERENCES Compra(IdCompra) ON DELETE CASCADE,
    FOREIGN KEY (ProductoId) REFERENCES Producto(IdProducto),
    CHECK (Cantidad > 0)
);

CREATE TABLE DetalleDistribucion (
    IdDetalleDistribucion INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    DistribucionId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioCostoUnitario DECIMAL(10, 2),
    Subtotal DECIMAL(10, 2) AS (Cantidad * PrecioCostoUnitario) STORED,
    FOREIGN KEY (DistribucionId) REFERENCES Distribucion(IdDistribucion) ON DELETE CASCADE,
    FOREIGN KEY (ProductoId) REFERENCES Producto(IdProducto),
    CHECK (Cantidad > 0)
);

-- 4. TRIGGERS DE COMPRAS
DELIMITER //

CREATE TRIGGER trg_compra_after_insert
AFTER INSERT ON DetalleCompra FOR EACH ROW
BEGIN
    UPDATE Producto SET CantidadStock = CantidadStock + NEW.Cantidad, Precio = NEW.PrecioUnitarioCosto, Estado = 'Disponible' WHERE IdProducto = NEW.ProductoId;
    UPDATE Compra SET MontoTotalCompra = MontoTotalCompra + NEW.Subtotal WHERE IdCompra = NEW.CompraId;
END //

CREATE TRIGGER trg_compra_after_update
AFTER UPDATE ON DetalleCompra FOR EACH ROW
BEGIN
    UPDATE Producto SET CantidadStock = (CantidadStock - OLD.Cantidad) + NEW.Cantidad, Precio = NEW.PrecioUnitarioCosto,
    Estado = IF(((CantidadStock - OLD.Cantidad) + NEW.Cantidad) > 0, 'Disponible', 'Agotado') WHERE IdProducto = NEW.ProductoId;
    UPDATE Compra SET MontoTotalCompra = (MontoTotalCompra - OLD.Subtotal) + NEW.Subtotal WHERE IdCompra = NEW.CompraId;
END //

CREATE TRIGGER trg_compra_after_delete
AFTER DELETE ON DetalleCompra FOR EACH ROW
BEGIN
    UPDATE Producto SET CantidadStock = CantidadStock - OLD.Cantidad, Estado = IF((CantidadStock - OLD.Cantidad) <= 0, 'Agotado', 'Disponible') WHERE IdProducto = OLD.ProductoId;
    UPDATE Compra SET MontoTotalCompra = MontoTotalCompra - OLD.Subtotal WHERE IdCompra = OLD.CompraId;
END //

-- 5. TRIGGERS DE DISTRIBUCIÓN
CREATE TRIGGER trg_dist_before_insert
BEFORE INSERT ON DetalleDistribucion FOR EACH ROW
BEGIN
    DECLARE v_stock INT; DECLARE v_precio DECIMAL(10,2);
    SELECT CantidadStock, Precio INTO v_stock, v_precio FROM Producto WHERE IdProducto = NEW.ProductoId FOR UPDATE;
    IF v_stock < NEW.Cantidad THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: Stock insuficiente.'; END IF;
    SET NEW.PrecioCostoUnitario = v_precio;
END //

CREATE TRIGGER trg_dist_before_update
BEFORE UPDATE ON DetalleDistribucion FOR EACH ROW
BEGIN
    DECLARE v_stock_actual INT;
    DECLARE v_precio_act DECIMAL(10,2);
    SELECT CantidadStock, Precio INTO v_stock_actual, v_precio_act FROM Producto WHERE IdProducto = NEW.ProductoId FOR UPDATE;
    IF (v_stock_actual + OLD.Cantidad) < NEW.Cantidad THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: No hay suficiente stock.';
    END IF;
    SET NEW.PrecioCostoUnitario = v_precio_act;
END //

CREATE TRIGGER trg_dist_after_insert AFTER INSERT ON DetalleDistribucion FOR EACH ROW
BEGIN
    UPDATE Producto SET CantidadStock = CantidadStock - NEW.Cantidad, Estado = IF(CantidadStock <= 0, 'Agotado', 'Disponible') WHERE IdProducto = NEW.ProductoId;
    UPDATE Distribucion SET MontoTotalDistribucion = MontoTotalDistribucion + NEW.Subtotal WHERE IdDistribucion = NEW.DistribucionId;
END //

CREATE TRIGGER trg_dist_after_update AFTER UPDATE ON DetalleDistribucion FOR EACH ROW
BEGIN
    UPDATE Producto SET CantidadStock = (CantidadStock + OLD.Cantidad) - NEW.Cantidad, Estado = IF(((CantidadStock + OLD.Cantidad) - NEW.Cantidad) <= 0, 'Agotado', 'Disponible') WHERE IdProducto = NEW.ProductoId;
    UPDATE Distribucion SET MontoTotalDistribucion = (MontoTotalDistribucion - OLD.Subtotal) + NEW.Subtotal WHERE IdDistribucion = NEW.DistribucionId;
END //

CREATE TRIGGER trg_dist_after_delete AFTER DELETE ON DetalleDistribucion FOR EACH ROW
BEGIN
    UPDATE Producto SET CantidadStock = CantidadStock + OLD.Cantidad, Estado = 'Disponible' WHERE IdProducto = OLD.ProductoId;
    UPDATE Distribucion SET MontoTotalDistribucion = MontoTotalDistribucion - OLD.Subtotal WHERE IdDistribucion = OLD.DistribucionId;
END //

DELIMITER ;