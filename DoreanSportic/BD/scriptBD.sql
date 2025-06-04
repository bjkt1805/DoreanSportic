USE DoreanSportic
GO

IF DB_NAME() <> N'DoreanSportic' SET NOEXEC ON
GO

--
-- Drop table [dbo].[Carrito_Detalle]
--
PRINT (N'Drop table [dbo].[Carrito_Detalle]')
GO
DROP TABLE dbo.Carrito_Detalle
GO

--
-- Drop table [dbo].[Carrito]
--
PRINT (N'Drop table [dbo].[Carrito]')
GO
DROP TABLE dbo.Carrito
GO

--
-- Drop table [dbo].[Pedido_Detalle]
--
PRINT (N'Drop table [dbo].[Pedido_Detalle]')
GO
DROP TABLE dbo.Pedido_Detalle
GO

--
-- Drop table [dbo].[Producto_Personalizacion]
--
PRINT (N'Drop table [dbo].[Producto_Personalizacion]')
GO
DROP TABLE dbo.Producto_Personalizacion
GO

--
-- Drop table [dbo].[Producto]
--
PRINT (N'Drop table [dbo].[Producto]')
GO
DROP TABLE dbo.Producto
GO

--
-- Drop table [dbo].[Categoria]
--
PRINT (N'Drop table [dbo].[Categoria]')
GO
DROP TABLE dbo.Categoria
GO

--
-- Drop table [dbo].[Color]
--
PRINT (N'Drop table [dbo].[Color]')
GO
DROP TABLE dbo.Color
GO

--
-- Drop table [dbo].[Empaque]
--
PRINT (N'Drop table [dbo].[Empaque]')
GO
DROP TABLE dbo.Empaque
GO

--
-- Drop table [dbo].[Estampado]
--
PRINT (N'Drop table [dbo].[Estampado]')
GO
DROP TABLE dbo.Estampado
GO

--
-- Drop table [dbo].[Marca]
--
PRINT (N'Drop table [dbo].[Marca]')
GO
DROP TABLE dbo.Marca
GO

--
-- Drop table [dbo].[Usuario]
--
PRINT (N'Drop table [dbo].[Usuario]')
GO
DROP TABLE dbo.Usuario
GO

--
-- Drop table [dbo].[Rol]
--
PRINT (N'Drop table [dbo].[Rol]')
GO
DROP TABLE dbo.Rol
GO

--
-- Drop table [dbo].[Pedido]
--
PRINT (N'Drop table [dbo].[Pedido]')
GO
DROP TABLE dbo.Pedido
GO

--
-- Drop table [dbo].[Cliente]
--
PRINT (N'Drop table [dbo].[Cliente]')
GO
DROP TABLE dbo.Cliente
GO

--
-- Drop table [dbo].[Sexo]
--
PRINT (N'Drop table [dbo].[Sexo]')
GO
DROP TABLE dbo.Sexo
GO

--
-- Create table [dbo].[Sexo]
--
PRINT (N'Create table [dbo].[Sexo]')
GO
CREATE TABLE dbo.Sexo (
  id int IDENTITY,
  nombre nvarchar(50) NOT NULL,
  PRIMARY KEY CLUSTERED (id)
)
ON [PRIMARY]
GO

--
-- Create table [dbo].[Cliente]
--
PRINT (N'Create table [dbo].[Cliente]')
GO
CREATE TABLE dbo.Cliente (
  id int IDENTITY,
  nombre nvarchar(100) NOT NULL,
  apellido nvarchar(100) NOT NULL,
  email nvarchar(200) NOT NULL,
  idSexo int NULL,
  edad int NULL,
  telefono nvarchar(20) NULL,
  direccionEnvio nvarchar(500) NULL,
  estado bit NOT NULL,
  PRIMARY KEY CLUSTERED (id),
  UNIQUE (email)
)
ON [PRIMARY]
GO

--
-- Create foreign key [FK_Cliente_Sexo] on table [dbo].[Cliente]
--
PRINT (N'Create foreign key [FK_Cliente_Sexo] on table [dbo].[Cliente]')
GO
ALTER TABLE dbo.Cliente
  ADD CONSTRAINT FK_Cliente_Sexo FOREIGN KEY (idSexo) REFERENCES dbo.Sexo (id)
GO

--
-- Create table [dbo].[Pedido]
--
PRINT (N'Create table [dbo].[Pedido]')
GO
CREATE TABLE dbo.Pedido (
  id int IDENTITY,
  idCliente int NOT NULL,
  fechaPedido datetime NOT NULL DEFAULT (getdate()),
  estadoPedido nvarchar(50) NOT NULL DEFAULT ('Pendiente'),
  total decimal(12, 2) NOT NULL,
  idMetodoPago int NULL,
  estado bit NOT NULL,
  PRIMARY KEY CLUSTERED (id)
)
ON [PRIMARY]
GO

--
-- Create foreign key [FK_Pedido_Cliente] on table [dbo].[Pedido]
--
PRINT (N'Create foreign key [FK_Pedido_Cliente] on table [dbo].[Pedido]')
GO
ALTER TABLE dbo.Pedido
  ADD CONSTRAINT FK_Pedido_Cliente FOREIGN KEY (idCliente) REFERENCES dbo.Cliente (id)
GO

--
-- Create foreign key [FK_Pedido_MetodoPago] on table [dbo].[Pedido]
--
PRINT (N'Create foreign key [FK_Pedido_MetodoPago] on table [dbo].[Pedido]')
GO
ALTER TABLE dbo.Pedido
  ADD CONSTRAINT FK_Pedido_MetodoPago FOREIGN KEY (idMetodoPago) REFERENCES dbo.MetodoPago (id)
GO

--
-- Create table [dbo].[Rol]
--
PRINT (N'Create table [dbo].[Rol]')
GO
CREATE TABLE dbo.Rol (
  id int IDENTITY,
  nombre nvarchar(30) NOT NULL,
  estado bit NOT NULL,
  PRIMARY KEY CLUSTERED (id)
)
ON [PRIMARY]
GO

--
-- Create table [dbo].[Usuario]
--
PRINT (N'Create table [dbo].[Usuario]')
GO
CREATE TABLE dbo.Usuario (
  id int IDENTITY,
  idCliente int NOT NULL,
  userName nvarchar(100) NOT NULL,
  passwordHash nvarchar(200) NOT NULL,
  fechaRegistro datetime NOT NULL DEFAULT (getdate()),
  esActivo bit NOT NULL DEFAULT (1),
  estado bit NOT NULL,
  idRol int NOT NULL,
  PRIMARY KEY CLUSTERED (id),
  UNIQUE (userName),
  UNIQUE (idCliente)
)
ON [PRIMARY]
GO

--
-- Create foreign key [FK_Rol] on table [dbo].[Usuario]
--
PRINT (N'Create foreign key [FK_Rol] on table [dbo].[Usuario]')
GO
ALTER TABLE dbo.Usuario
  ADD CONSTRAINT FK_Rol FOREIGN KEY (idRol) REFERENCES dbo.Rol (id)
GO

--
-- Create foreign key [FK_Usuario_Cliente] on table [dbo].[Usuario]
--
PRINT (N'Create foreign key [FK_Usuario_Cliente] on table [dbo].[Usuario]')
GO
ALTER TABLE dbo.Usuario
  ADD CONSTRAINT FK_Usuario_Cliente FOREIGN KEY (idCliente) REFERENCES dbo.Cliente (id)
GO

--
-- Create table [dbo].[Marca]
--
PRINT (N'Create table [dbo].[Marca]')
GO
CREATE TABLE dbo.Marca (
  id int IDENTITY,
  nombre nvarchar(100) NOT NULL,
  foto varbinary(max) NULL,
  estado bit NOT NULL,
  PRIMARY KEY CLUSTERED (id)
)
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO

--
-- Create table [dbo].[Estampado]
--
PRINT (N'Create table [dbo].[Estampado]')
GO
CREATE TABLE dbo.Estampado (
  id int IDENTITY,
  nombre nvarchar(100) NOT NULL,
  descripcion nvarchar(max) NULL,
  tipo nvarchar(50) NOT NULL,
  estado bit NOT NULL,
  foto varbinary(max) NOT NULL,
  PRIMARY KEY CLUSTERED (id)
)
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO

--
-- Create table [dbo].[Empaque]
--
PRINT (N'Create table [dbo].[Empaque]')
GO
CREATE TABLE dbo.Empaque (
  id int IDENTITY,
  tipoEmpaque nvarchar(50) NOT NULL,
  descripcion nvarchar(max) NULL,
  estado bit NOT NULL,
  PRIMARY KEY CLUSTERED (id)
)
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO

--
-- Create table [dbo].[Color]
--
PRINT (N'Create table [dbo].[Color]')
GO
CREATE TABLE dbo.Color (
  id int IDENTITY,
  colorBase nvarchar(50) NOT NULL,
  saturacion nvarchar(50) NOT NULL,
  estado bit NOT NULL,
  PRIMARY KEY CLUSTERED (id)
)
ON [PRIMARY]
GO

--
-- Create table [dbo].[Categoria]
--
PRINT (N'Create table [dbo].[Categoria]')
GO
CREATE TABLE dbo.Categoria (
  id int IDENTITY,
  nombre nvarchar(100) NOT NULL,
  estado bit NOT NULL,
  PRIMARY KEY CLUSTERED (id)
)
ON [PRIMARY]
GO

--
-- Create table [dbo].[Producto]
--
PRINT (N'Create table [dbo].[Producto]')
GO
CREATE TABLE dbo.Producto (
  id int IDENTITY,
  nombre nvarchar(200) NOT NULL,
  descripcion nvarchar(max) NULL,
  precioBase decimal(10, 2) NOT NULL,
  stock int NOT NULL,
  idMarca int NOT NULL,
  idCategoria int NOT NULL,
  foto varbinary(max) NULL,
  estado bit NOT NULL,
  PRIMARY KEY CLUSTERED (id)
)
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO

--
-- Create foreign key [FK_Producto_Categoria] on table [dbo].[Producto]
--
PRINT (N'Create foreign key [FK_Producto_Categoria] on table [dbo].[Producto]')
GO
ALTER TABLE dbo.Producto
  ADD CONSTRAINT FK_Producto_Categoria FOREIGN KEY (idCategoria) REFERENCES dbo.Categoria (id)
GO

--
-- Create foreign key [FK_Producto_Marca] on table [dbo].[Producto]
--
PRINT (N'Create foreign key [FK_Producto_Marca] on table [dbo].[Producto]')
GO
ALTER TABLE dbo.Producto
  ADD CONSTRAINT FK_Producto_Marca FOREIGN KEY (idMarca) REFERENCES dbo.Marca (id)
GO

--
-- Create table [dbo].[Producto_Personalizacion]
--
PRINT (N'Create table [dbo].[Producto_Personalizacion]')
GO
CREATE TABLE dbo.Producto_Personalizacion (
  id int IDENTITY,
  idProducto int NOT NULL,
  idColor int NOT NULL,
  idEstampado int NOT NULL,
  idEmpaque int NOT NULL,
  precioFinal decimal(10, 2) NOT NULL,
  estado bit NOT NULL,
  PRIMARY KEY CLUSTERED (id)
)
ON [PRIMARY]
GO

--
-- Create foreign key [FK_ProdPers_Color] on table [dbo].[Producto_Personalizacion]
--
PRINT (N'Create foreign key [FK_ProdPers_Color] on table [dbo].[Producto_Personalizacion]')
GO
ALTER TABLE dbo.Producto_Personalizacion
  ADD CONSTRAINT FK_ProdPers_Color FOREIGN KEY (idColor) REFERENCES dbo.Color (id)
GO

--
-- Create foreign key [FK_ProdPers_Empaque] on table [dbo].[Producto_Personalizacion]
--
PRINT (N'Create foreign key [FK_ProdPers_Empaque] on table [dbo].[Producto_Personalizacion]')
GO
ALTER TABLE dbo.Producto_Personalizacion
  ADD CONSTRAINT FK_ProdPers_Empaque FOREIGN KEY (idEmpaque) REFERENCES dbo.Empaque (id)
GO

--
-- Create foreign key [FK_ProdPers_Estampado] on table [dbo].[Producto_Personalizacion]
--
PRINT (N'Create foreign key [FK_ProdPers_Estampado] on table [dbo].[Producto_Personalizacion]')
GO
ALTER TABLE dbo.Producto_Personalizacion
  ADD CONSTRAINT FK_ProdPers_Estampado FOREIGN KEY (idEstampado) REFERENCES dbo.Estampado (id)
GO

--
-- Create foreign key [FK_ProdPers_Producto] on table [dbo].[Producto_Personalizacion]
--
PRINT (N'Create foreign key [FK_ProdPers_Producto] on table [dbo].[Producto_Personalizacion]')
GO
ALTER TABLE dbo.Producto_Personalizacion
  ADD CONSTRAINT FK_ProdPers_Producto FOREIGN KEY (idProducto) REFERENCES dbo.Producto (id)
GO

--
-- Create table [dbo].[Pedido_Detalle]
--
PRINT (N'Create table [dbo].[Pedido_Detalle]')
GO
CREATE TABLE dbo.Pedido_Detalle (
  id int IDENTITY,
  idPedido int NOT NULL,
  idProducto int NOT NULL,
  idColor int NOT NULL,
  idEstampado int NOT NULL,
  idEmpaque int NOT NULL,
  cantidad int NOT NULL,
  precioUnitario decimal(10, 2) NOT NULL,
  subTotal decimal(12, 2) NOT NULL,
  estado bit NOT NULL,
  PRIMARY KEY CLUSTERED (id)
)
ON [PRIMARY]
GO

--
-- Create foreign key [FK_PedidoDetalle_Color] on table [dbo].[Pedido_Detalle]
--
PRINT (N'Create foreign key [FK_PedidoDetalle_Color] on table [dbo].[Pedido_Detalle]')
GO
ALTER TABLE dbo.Pedido_Detalle
  ADD CONSTRAINT FK_PedidoDetalle_Color FOREIGN KEY (idColor) REFERENCES dbo.Color (id)
GO

--
-- Create foreign key [FK_PedidoDetalle_Empaque] on table [dbo].[Pedido_Detalle]
--
PRINT (N'Create foreign key [FK_PedidoDetalle_Empaque] on table [dbo].[Pedido_Detalle]')
GO
ALTER TABLE dbo.Pedido_Detalle
  ADD CONSTRAINT FK_PedidoDetalle_Empaque FOREIGN KEY (idEmpaque) REFERENCES dbo.Empaque (id)
GO

--
-- Create foreign key [FK_PedidoDetalle_Estampado] on table [dbo].[Pedido_Detalle]
--
PRINT (N'Create foreign key [FK_PedidoDetalle_Estampado] on table [dbo].[Pedido_Detalle]')
GO
ALTER TABLE dbo.Pedido_Detalle
  ADD CONSTRAINT FK_PedidoDetalle_Estampado FOREIGN KEY (idEstampado) REFERENCES dbo.Estampado (id)
GO

--
-- Create foreign key [FK_PedidoDetalle_Pedido] on table [dbo].[Pedido_Detalle]
--
PRINT (N'Create foreign key [FK_PedidoDetalle_Pedido] on table [dbo].[Pedido_Detalle]')
GO
ALTER TABLE dbo.Pedido_Detalle
  ADD CONSTRAINT FK_PedidoDetalle_Pedido FOREIGN KEY (idPedido) REFERENCES dbo.Pedido (id)
GO

--
-- Create foreign key [FK_PedidoDetalle_Producto] on table [dbo].[Pedido_Detalle]
--
PRINT (N'Create foreign key [FK_PedidoDetalle_Producto] on table [dbo].[Pedido_Detalle]')
GO
ALTER TABLE dbo.Pedido_Detalle
  ADD CONSTRAINT FK_PedidoDetalle_Producto FOREIGN KEY (idProducto) REFERENCES dbo.Producto (id)
GO

--
-- Create table [dbo].[Carrito]
--
PRINT (N'Create table [dbo].[Carrito]')
GO
CREATE TABLE dbo.Carrito (
  id int IDENTITY,
  idCliente int NOT NULL,
  fechaCreacion datetime NOT NULL DEFAULT (getdate()),
  estadoPago nvarchar(20) NOT NULL,
  estado bit NOT NULL,
  PRIMARY KEY CLUSTERED (id)
)
ON [PRIMARY]
GO

--
-- Create table [dbo].[Carrito_Detalle]
--
PRINT (N'Create table [dbo].[Carrito_Detalle]')
GO
CREATE TABLE dbo.Carrito_Detalle (
  id int IDENTITY,
  idCarrito int NOT NULL,
  idProducto int NOT NULL,
  idColor int NOT NULL,
  idEstampado int NOT NULL,
  idEmpaque int NOT NULL,
  cantidad int NOT NULL,
  estado bit NOT NULL,
  PRIMARY KEY CLUSTERED (id)
)
ON [PRIMARY]
GO

--
-- Create foreign key [FK_CarritoDetalle_Carrito] on table [dbo].[Carrito_Detalle]
--
PRINT (N'Create foreign key [FK_CarritoDetalle_Carrito] on table [dbo].[Carrito_Detalle]')
GO
ALTER TABLE dbo.Carrito_Detalle
  ADD CONSTRAINT FK_CarritoDetalle_Carrito FOREIGN KEY (idCarrito) REFERENCES dbo.Carrito (id)
GO

--
-- Create foreign key [FK_CarritoDetalle_Color] on table [dbo].[Carrito_Detalle]
--
PRINT (N'Create foreign key [FK_CarritoDetalle_Color] on table [dbo].[Carrito_Detalle]')
GO
ALTER TABLE dbo.Carrito_Detalle
  ADD CONSTRAINT FK_CarritoDetalle_Color FOREIGN KEY (idColor) REFERENCES dbo.Color (id)
GO

--
-- Create foreign key [FK_CarritoDetalle_Empaque] on table [dbo].[Carrito_Detalle]
--
PRINT (N'Create foreign key [FK_CarritoDetalle_Empaque] on table [dbo].[Carrito_Detalle]')
GO
ALTER TABLE dbo.Carrito_Detalle
  ADD CONSTRAINT FK_CarritoDetalle_Empaque FOREIGN KEY (idEmpaque) REFERENCES dbo.Empaque (id)
GO

--
-- Create foreign key [FK_CarritoDetalle_Estampado] on table [dbo].[Carrito_Detalle]
--
PRINT (N'Create foreign key [FK_CarritoDetalle_Estampado] on table [dbo].[Carrito_Detalle]')
GO
ALTER TABLE dbo.Carrito_Detalle
  ADD CONSTRAINT FK_CarritoDetalle_Estampado FOREIGN KEY (idEstampado) REFERENCES dbo.Estampado (id)
GO

--
-- Create foreign key [FK_CarritoDetalle_Producto] on table [dbo].[Carrito_Detalle]
--
PRINT (N'Create foreign key [FK_CarritoDetalle_Producto] on table [dbo].[Carrito_Detalle]')
GO
ALTER TABLE dbo.Carrito_Detalle
  ADD CONSTRAINT FK_CarritoDetalle_Producto FOREIGN KEY (idProducto) REFERENCES dbo.Producto (id)
GO

SET NOEXEC OFF
GO