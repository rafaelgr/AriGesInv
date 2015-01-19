#--- Obtencion del artículo con todos sus datos.
SELECT 
art.codartic AS CODIGO_ARTICULO,
COALESCE(ar3.codigoea,'') AS CODIGO_EAN,
COALESCE(alm.statusin, 0) AS STATUS,
art.nomartic AS NOMBRE_ARTICULO,
art.codigiva AS CODIGO_IVA,
art.preciove AS PRECIO_SIN_IVA,
COALESCE(art.preciomp,0) AS PRECIOMP_ARTICULO,
COALESCE(art.precioma,0) AS PRECIOMA_ARTICULO,
COALESCE(art.preciouc,0) AS PRECIOUC_ARTICULO,
COALESCE(art.preciost,0) AS PRECIOST_ARTICULO,
alm.codalmac AS CODIGO_ALMACEN,
alp.nomalmac AS NOMBRE_ALMACEN,
alm.canstock AS STOCK
FROM sartic AS art
LEFT JOIN sarti3 AS ar3 ON ar3.codartic = art.codartic
LEFT JOIN salmac AS alm ON alm.codartic = art.codartic
LEFT JOIN salmpr AS alp ON alp.codalmac = alm.codalmac;

#-------------- Antes de actualizar el stock hay que grabar la shinve.
INSERT IGNORE INTO shinve(codartic, codalmac, fechainv, horainve, existenc)
SELECT codartic, codalmac, fechainv, horainve, stockinv FROM salmac
WHERE codartic = 'VALOR' AND codalmac = 'VALOR';

#-------------- Actualizar la tabla de movimentos
INSERT INTO smoval (codartic, codalmac, fechamov, horamovi, tipomovi, detamovi, impormov, codigope, letraser, documento, numlinea)
VALUES (1,2,3,4,5,6,7,8,9,10,11);
/* 
5 - tipomovi (0=Salida / 1=Entrada)
6 - detamovi ('DFI')
7 - impormov (diferencia por precio de coste) (PRECIOUC_ARTICULO)
8 - codigope (código del operario conectado) (straba.codtraba)
9 - letraser (vacío)
10 - documento ('LECTOR')
11 - numlinea (fijo 1)
*/
#---------------- Actualización de la salmac
UPDATE salmac SET 
canstock = 'VALOR', statusin=0, stockinv='VALOR', fechainv='VALOR', horainve='VALOR'
WHERE codartic = 'VALOR' AND codalmac = 'VALOR';

# Final de transacción.

# -- ver si hay trabajador en la straba
SELECT codtraba FROM straba WHERE login = 'root';

#-- calcular el iva
SELECT porceiva FROM tiposiva WHERE codigiva = 1;
