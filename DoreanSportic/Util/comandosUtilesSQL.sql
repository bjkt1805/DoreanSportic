-- AÑADIR LOS CONSTRAINTS (LLAVES FORÁNEAS)

ALTER TABLE Ordenes
ADD CONSTRAINT FK_Ordenes_Clientes
FOREIGN KEY (ClienteID)
REFERENCES Clientes(ClienteID)
ON DELETE CASCADE
ON UPDATE CASCADE;

-- REVISAR TODOS LOS CONSTRAINTS DE LA BASE DE DATOS 

SELECT 
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumn,
    fk.delete_referential_action_desc AS OnDeleteAction,
    fk.update_referential_action_desc AS OnUpdateAction
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
ORDER BY TableName, ForeignKeyName;

-- RESETEAR EL IDENTITY A 1 (SIEMPRE QUE NO EXISTAN REGISTROS EN LA TABLA)
DBCC CHECKIDENT ('NombreTabla', RESEED, 0);

-- ACTUALIZAR LA FOTO DEL PRODUCTO (CAMBIAR ID DE SER NECESARIO)
UPDATE ImagenProducto
SET imagen = (
    SELECT BulkData
    FROM OPENROWSET(
        BULK 'C:\TEMP\',
        SINGLE_BLOB
    ) AS Imagen (BulkData)
)
WHERE id = 1;

-- AGREGAR UN CONSTRAINT PARA ACEPTAR VALORES SOLO ENTRE 1 Y 5 -- 

ALTER TABLE ResennaValoracion
ADD CONSTRAINT chk_columna_rango
CHECK (calificacion BETWEEN 1 AND 5);

-- CREAR UN TRIGGER PARA QUE EL ID DE PEDIDO SEA YYYYMMDDhhmmss

CREATE TRIGGER trg_GenerarIdPedido
ON Pedido
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @fechaActual VARCHAR(14) = CONVERT(VARCHAR(14), GETDATE(), 112) + 
                                       RIGHT('0' + CAST(DATEPART(HOUR, GETDATE()) AS VARCHAR), 2) + 
                                       RIGHT('0' + CAST(DATEPART(MINUTE, GETDATE()) AS VARCHAR), 2) + 
                                       RIGHT('0' + CAST(DATEPART(SECOND, GETDATE()) AS VARCHAR), 2);

    INSERT INTO Pedido (
        id, idCliente, estadoPedido, fechaPedido, estado, total, idMetodoPago
    )
    SELECT 
        @fechaActual, idCliente, estadoPedido, fechaPedido, estado, total, idMetodoPago
    FROM inserted;
END;




