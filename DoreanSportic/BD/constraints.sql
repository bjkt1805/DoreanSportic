Use DoreanSportic;

alter table Tarjeta
add CONSTRAINT FK_Tarjeta_Cliente FOREIGN KEY (idCliente) REFERENCES Cliente(id)


alter table Usuario
add constraint FK_Usuario_Cliente FOREIGN KEY (idCliente) REFERENCES Cliente(id);

Alter table Producto
add constraint FK_Producto_Marca FOREIGN KEY (idMarca) REFERENCES Marca(id);

Alter table Producto
add constraint FK_Producto_Categoria FOREIGN KEY (idCategoria) REFERENCES Categoria(id)

Alter table Producto_Personalizacion
add constraint FK_ProdPers_Producto FOREIGN KEY (idProducto) REFERENCES Producto(id)

Alter table Producto_Personalizacion
add constraint FK_ProdPers_Color FOREIGN KEY (idColor) REFERENCES Color(id)

Alter table Producto_Personalizacion
add constraint FK_ProdPers_Estampado FOREIGN KEY (idEstampado) REFERENCES Estampado(id)

Alter table Producto_Personalizacion
add constraint FK_ProdPers_Empaque FOREIGN KEY (idEmpaque) REFERENCES Empaque(id)

Alter table Cliente
add constraint FK_Cliente_Sexo FOREIGN KEY (idSexo) REFERENCES Sexo(id);

Alter table Pedido
add constraint FK_Pedido_Cliente FOREIGN KEY (idCliente) REFERENCES Cliente(id)

Alter table Pedido_Detalle
add constraint FK_PedidoDetalle_Pedido FOREIGN KEY (idPedido) REFERENCES Pedido(id)

Alter table Pedido_Detalle
add constraint FK_PedidoDetalle_Producto FOREIGN KEY (idProducto) REFERENCES Producto(id)

Alter table Pedido_Detalle
add constraint FK_PedidoDetalle_Color FOREIGN KEY (idColor) REFERENCES Color(id)

Alter table Pedido_Detalle
add constraint FK_PedidoDetalle_Estampado FOREIGN KEY (idEstampado) REFERENCES Estampado(id)

Alter table Pedido_Detalle
add constraint FK_PedidoDetalle_Empaque FOREIGN KEY (idEmpaque) REFERENCES Empaque(id)

Alter table Carrito_Detalle
add constraint FK_CarritoDetalle_Carrito FOREIGN KEY (idCarrito) REFERENCES Carrito(id)

Alter table Carrito_Detalle
add constraint FK_CarritoDetalle_Producto FOREIGN KEY (idProducto) REFERENCES Producto(id)

Alter table Carrito_Detalle
add constraint FK_CarritoDetalle_Color FOREIGN KEY (idColor) REFERENCES Color(id)

Alter table Carrito_Detalle
add constraint FK_CarritoDetalle_Estampado FOREIGN KEY (idEstampado) REFERENCES Estampado(id)

Alter table Carrito_Detalle
add constraint FK_CarritoDetalle_Empaque FOREIGN KEY (idEmpaque) REFERENCES Empaque(id)

Alter table Usuario
add constraint FK_Rol FOREIGN KEY (idRol) REFERENCES Rol(id)












