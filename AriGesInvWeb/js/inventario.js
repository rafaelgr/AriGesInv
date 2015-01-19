// funciones correspondientes a la página inventario.html
var InvetarioApp = {};

(function (app) {
    // definición variables globales
    var $ean = $('#txtEan'),
        $cantidad = $('#txtCantidad'),
        $msg = $('#txtMsg'),
        $almacenes = $('#almacenes'),
        $stock = $('#txtStock'),
        $precio = $('#txtPrecio'),
        $nombre = $('#txtNombre')
    var datos = [];
    var almacenes = [];
    var almacen = function (codigo, nombre) {
        this.Codigo = codigo;
        this.Nombre = nombre;
    }
    // parametrización numeral
    numeral.language('es', {
        delimiters: {
            thousands: '.',
            decimal: ','
        }
    });
    numeral.language('es');
    // definición de funciones.
    app.int = function () {
        app.bindings();
    }
    app.bindings = function () {
        $almacenes.change(function () {
            // han cambiado de almacén y hay que mostrar
            // los datos correspondientes
            for (var i = 0; i < datos.length; i++) {
                if (datos[i].CodigoAlmacen == $almacenes.val()) {
                    $stock.text(numeral(datos[i].Stock).format('#,###,##0.00'));
                    $precio.text(numeral(datos[i].PrecioConIva).format('#,###,##0.00'));
                }
            }
        });
        $('#btnEan').click(function () {
            if (!app.validateFormEan())
                return;
            // leemos el artículo del que nos han proporcionado el EAN.
            var data = {
                "ean": $ean.val()
            }
            $.ajax({
                       type: "POST",
                       url: "InventarioApi.aspx/GetArticuloEan",
                       dataType: "json",
                       contentType: "application/json",
                       data: JSON.stringify(data),
                       success: function (data, status) {
                           // Regresa el mensaje
                           if (!data.d) {
                               // 
                               $msg.text('No existe ningún artículo con ese ean');
                               $.mobile.changePage('#pgMsg');
                           }
                           else {
                               // comprobamos que no se está inventariando
                               var articulo = data.d;
                               if (articulo.Status == 1) {
                                   //
                                   $msg.text('El artículo ' + articulo.NombreArticulo + ' se está inventariando ya');
                                   $.mobile.changePage('#pgMsg');
                               }
                               else {
                                   data = {
                                       "ean": $ean.val()
                                   }
                                   // ahora sí que vamos a buscar stock para inventariar.
                                   $.ajax({
                                              type: "POST",
                                              url: "InventarioApi.aspx/GetArticulosEan",
                                              dataType: "json",
                                              contentType: "application/json",
                                              data: JSON.stringify(data),
                                              success: function (data, status) {
                                                  // Regresa el mensaje
                                                  if (!data.d) {
                                                      // 
                                                      $msg.text('No hay datos de stock de este artículo');
                                                      $.mobile.changePage('#pgMsg');
                                                  }
                                                  else {
                                                      // antes de asignar iniciamos variables
                                                      datos = []; almacenes = [];
                                                      // montamos los datos
                                                      datos = data.d;
                                                      // ahora montamos los almacenes para el desplegable
                                                      var alHtml = "";
                                                      for (var i = 0; i < datos.length; i++) {
                                                          var al = new almacen(datos[i].CodigoAlmacen, datos[i].NombreAlmacen);
                                                          if (i == 0) {
                                                              alHtml += "<option selected value='" + al.Codigo + "'>" + al.Nombre + "</option>";
                                                          } else {
                                                              alHtml += "<option value='" + al.Codigo + "'>" + al.Nombre + "</option>";
                                                          }
                                                          almacenes.push(al);
                                                      }
                                                      // cargamos los valores del desplegable (por defecto seleccionado el 0)
                                                      $almacenes.html(alHtml);
                                                      // los valores a los campos correspondientes
                                                      $nombre.text(datos[0].NombreArticulo);
                                                      $stock.text(numeral(datos[0].Stock).format('#,###,##0.00'));
                                                      $precio.text(numeral(datos[0].PrecioConIva).format('#,###,##0.00'));
                                                      // mostramos la página
                                                      $.mobile.changePage('#pgArt');
                                                  }
                                              },
                                              error: function (xhr, textStatus, errorThrwon) {
                                                  var m = xhr.responseText;
                                                  if (!m)
                                                      m = "Error general posiblemente falla la conexión";
                                                  $msg.text(m);
                                                  $.mobile.changePage('#pgMsg');
                                              }
                                          });
                               }
                           }
                       },
                       error: function (xhr, textStatus, errorThrwon) {
                           var m = xhr.responseText;
                           if (!m)
                               m = "Error general posiblemente falla la conexión";
                           $msg.text(m);
                           $.mobile.changePage('#pgMsg');
                       }
                   });
            // leer los artículo y el stock
        });
        $('#btnActStock').click(function () {
            // actualizar el stock con la cantidad
        });
    }
    app.validateFormEan = function () {
        $('#frmEan').validate({
                                  rules: {
                ean: { required: true }
            },
                                  messages: {
                ean: {
                                          required: 'Introduzca un código de barras'
                                      }
            },
                                  errorPlacement: function (error, element) {
                                      error.insertAfter(element.parent());
                                  }
                              });
        return $('#frmEan').valid();
    }
    app.int();
})(InvetarioApp);