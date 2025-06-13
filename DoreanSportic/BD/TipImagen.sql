--MOSTRAR LA INFORMACION DE LA TABLA DE LIBRO (1000 REGISTROS)
SELECT TOP (1000) [IdLibro]
      ,[Isbn]
      ,[IdAutor]
      ,[Nombre]
      ,[Precio]
      ,[Cantidad]
      ,[Imagen]
  FROM [Libreria].[dbo].[Libro]

--INSERTAR DATO EN LA TABLA LIBRO (AGREGUE LOS VALORES A INSERTAR EN EL ORDEN INDICADO
--PARA EL TEMA DE LA IMAGEN COLOQUE LA RUTA EXACTA DÓNDE ESTARÁ LA IMAGEN)
INSERT INTO [dbo].[Libro]
           ([Isbn]
           ,[IdAutor]
           ,[Nombre]
           ,[Precio]
           ,[Cantidad]
           ,[Imagen])
     VALUES (
           '9788498974959'
           ,2
           ,'Ejemplo Prueba'
           ,21000
           ,8
           ,CONVERT(varbinary(max),(SELECT * FROM OPENROWSET(BULK 'C:\prueba.jpg', SINGLE_BLOB) AS image)));
GO

--SELECT PARA VER VALOR DE LA TABLA EN LA URL (CAMBIE EL VALOR DEL IDLIBRO A CONSULTAR)
SELECT 
  'data:image/jpeg;base64,' + CAST('' AS XML).value('xs:base64Binary(sql:column("libro.imagen"))', 'varchar(max)') AS image_base64
FROM libro where IdLibro=19;

-- ACTUALIZAR LA FOTO DEL PRODUCTO (CAMBIAR ID DE SER NECESARIO)
UPDATE Producto
SET foto = (
    SELECT BulkData
    FROM OPENROWSET(
        BULK 'C:\Users\1\OneDrive - Universidad Técnica Nacional\Respaldo_Brian\ISW\Programación en Ambiente Web II\Proyecto\DoreanSportic\DoreanSportic\wwwroot\Assets\tennis.webp',
        SINGLE_BLOB
    ) AS Imagen 
)
WHERE id = 1;