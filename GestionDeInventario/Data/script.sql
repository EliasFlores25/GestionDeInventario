-- =======================================================
-- 1. CONFIGURACIÓN INICIAL
-- =======================================================
SET default_storage_engine=InnoDB;

-- =======================================================
-- 2. CREACIÓN DE TABLAS
-- =======================================================

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
    IdProducto INT NOT NULL AUTO_INCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion TEXT,
    CantidadStock INT NOT NULL DEFAULT 0,
    UnidadMedida VARCHAR(20) NOT NULL,
    Precio DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    Estado ENUM('Disponible', 'Agotado') NOT NULL DEFAULT 'Agotado',
    PRIMARY KEY (IdProducto),
    UNIQUE INDEX idx_nombre_producto_unico (Nombre),
    CHECK (CantidadStock >= 0)
);

CREATE TABLE Usuario (
    IdUsuario INT NOT NULL AUTO_INCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    TipoRol ENUM('Administrador') NOT NULL,
    ContraseñaHash VARCHAR(255) NOT NULL,
    Estado ENUM('Activo', 'Inactivo') NOT NULL,
    PRIMARY KEY (IdUsuario)
);

CREATE TABLE Departamento (
    IdDepartamento INT NOT NULL AUTO_INCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion TEXT,
    PRIMARY KEY (IdDepartamento),
    UNIQUE INDEX idx_nombre_depto_unico (Nombre)
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

CREATE TABLE DetalleCompra (
    IdDetalleCompra INT NOT NULL AUTO_INCREMENT,
    NumeroFactura VARCHAR(50) NOT NULL,
    UsuarioId INT NOT NULL,
    ProveedorId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitarioCosto DECIMAL(10, 2) NOT NULL,
    MontoTotal DECIMAL(10, 2) AS (Cantidad * PrecioUnitarioCosto) STORED, 
    FechaCompra DATE NOT NULL,
    PRIMARY KEY (IdDetalleCompra),
    FOREIGN KEY (UsuarioId) REFERENCES Usuario(IdUsuario) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (ProveedorId) REFERENCES Proveedor(IdProveedor) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (ProductoId) REFERENCES Producto(IdProducto) ON DELETE RESTRICT ON UPDATE CASCADE,
    CHECK (Cantidad > 0)
);

CREATE TABLE DetalleDistribucion (
    IdDetalleDistribucion INT NOT NULL AUTO_INCREMENT,
    NumeroDistribucion VARCHAR(50) NOT NULL,
    UsuarioId INT NOT NULL,
    EmpleadoId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    FechaSalida DATE NOT NULL,
    Motivo VARCHAR(255),
    PrecioCostoUnitario DECIMAL(10, 2),
    MontoTotal DECIMAL(10, 2) AS (Cantidad * PrecioCostoUnitario) STORED,
    PRIMARY KEY (IdDetalleDistribucion),
    FOREIGN KEY (UsuarioId) REFERENCES Usuario(IdUsuario) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (EmpleadoId) REFERENCES Empleado(IdEmpleado) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (ProductoId) REFERENCES Producto(IdProducto) ON DELETE RESTRICT ON UPDATE CASCADE,
    CHECK (Cantidad > 0)
);

-- =======================================================
-- 3. TRIGGERS ACTUALIZADOS (LÓGICA AUTOMÁTICA)
-- =======================================================
DELIMITER //

-- -----------------------------------------------------
-- FLUJO DE COMPRAS (ENTRADAS)
-- -----------------------------------------------------

-- T-1: Al insertar compra: Aumenta stock, actualiza precio en Producto y cambia estado
CREATE TRIGGER trg_aumentar_stock_compra
AFTER INSERT ON DetalleCompra
FOR EACH ROW
BEGIN
    UPDATE Producto
    SET CantidadStock = CantidadStock + NEW.Cantidad,
        Precio = NEW.PrecioUnitarioCosto, -- (Req 2) Actualiza precio del producto
        Estado = 'Disponible'             -- (Req 1) Asegura disponibilidad
    WHERE IdProducto = NEW.ProductoId;
END //

-- T-2: Al actualizar compra
CREATE TRIGGER trg_actualizar_stock_compra
AFTER UPDATE ON DetalleCompra
FOR EACH ROW
BEGIN
    UPDATE Producto
    SET CantidadStock = CantidadStock - OLD.Cantidad + NEW.Cantidad,
        Precio = NEW.PrecioUnitarioCosto,
        Estado = IF((CantidadStock - OLD.Cantidad + NEW.Cantidad) > 0, 'Disponible', 'Agotado')
    WHERE IdProducto = NEW.ProductoId;
END //

-- T-3: Al eliminar compra
CREATE TRIGGER trg_eliminar_stock_compra
AFTER DELETE ON DetalleCompra
FOR EACH ROW
BEGIN
    UPDATE Producto
    SET CantidadStock = CantidadStock - OLD.Cantidad,
        Estado = IF((CantidadStock - OLD.Cantidad) <= 0, 'Agotado', 'Disponible')
    WHERE IdProducto = OLD.ProductoId;
END //


-- -----------------------------------------------------
-- FLUJO DE DISTRIBUCIÓN (SALIDAS)
-- -----------------------------------------------------

-- T-4: Antes de insertar: Valida stock y ASIGNA PRECIO automático
CREATE TRIGGER trg_validar_stock_distribucion
BEFORE INSERT ON DetalleDistribucion
FOR EACH ROW
BEGIN
    DECLARE stock_actual INT;
    DECLARE precio_ref DECIMAL(10,2);

    -- Bloqueo y obtención de datos actuales
    SELECT CantidadStock, Precio INTO stock_actual, precio_ref
    FROM Producto
    WHERE IdProducto = NEW.ProductoId
    FOR UPDATE;

    -- Validación de stock
    IF stock_actual < NEW.Cantidad THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Error: No hay suficiente stock disponible.';
    END IF; 

    -- (Req 3) Asigna el precio del producto a la distribución automáticamente
    SET NEW.PrecioCostoUnitario = precio_ref;
END //

-- T-5: Después de insertar: Disminuye stock y actualiza estado si llega a 0
CREATE TRIGGER trg_disminuir_stock_distribucion
AFTER INSERT ON DetalleDistribucion
FOR EACH ROW
BEGIN
    UPDATE Producto
    SET CantidadStock = CantidadStock - NEW.Cantidad,
        Estado = IF((CantidadStock - NEW.Cantidad) <= 0, 'Agotado', 'Disponible') -- (Req 1)
    WHERE IdProducto = NEW.ProductoId;
END //

-- T-6: Antes de actualizar distribución (Validación)
CREATE TRIGGER trg_validar_stock_update_distribucion
BEFORE UPDATE ON DetalleDistribucion
FOR EACH ROW
BEGIN
    DECLARE stock_temporal INT;
    IF OLD.Cantidad <> NEW.Cantidad THEN
        SELECT CantidadStock + OLD.Cantidad INTO stock_temporal
        FROM Producto
        WHERE IdProducto = OLD.ProductoId;

        IF stock_temporal < NEW.Cantidad THEN
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Error: La nueva cantidad excede el stock disponible.';
        END IF;
    END IF;
END //

-- T-7: Después de actualizar distribución
CREATE TRIGGER trg_actualizar_stock_distribucion
AFTER UPDATE ON DetalleDistribucion
FOR EACH ROW
BEGIN
    UPDATE Producto
    SET CantidadStock = CantidadStock + OLD.Cantidad - NEW.Cantidad,
        Estado = IF((CantidadStock + OLD.Cantidad - NEW.Cantidad) <= 0, 'Agotado', 'Disponible')
    WHERE IdProducto = NEW.ProductoId;
END //

DELIMITER ;