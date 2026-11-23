-- Solo consultas: no altera ni borra datos.
USE tienda_scaffolding;
SHOW TABLES;
SELECT TABLE_NAME, COUNT(*) AS cantidad_atributos
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'tienda_scaffolding'
GROUP BY TABLE_NAME ORDER BY TABLE_NAME;
SELECT * FROM categorias;
SELECT * FROM clientes;
SELECT * FROM productos;
SELECT * FROM pedidos;
SELECT * FROM detallespedido;
