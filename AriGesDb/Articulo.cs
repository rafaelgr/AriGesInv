using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriGesDb
{
    public class Articulo
    {
        private string codigoArticulo;

        public string CodigoArticulo
        {
            get { return codigoArticulo; }
            set { codigoArticulo = value; }
        }
        private string codigoEan;

        public string CodigoEan
        {
            get { return codigoEan; }
            set { codigoEan = value; }
        }
        private int status;

        public int Status
        {
            get { return status; }
            set { status = value; }
        }
        private string nombreArticulo;

        public string NombreArticulo
        {
            get { return nombreArticulo; }
            set { nombreArticulo = value; }
        }
        private int codigoIva;

        public int CodigoIva
        {
            get { return codigoIva; }
            set { codigoIva = value; }
        }
        private decimal precioSinIva;

        public decimal PrecioSinIva
        {
            get { return precioSinIva; }
            set { precioSinIva = value; }
        }
        private decimal precioConIva;

        public decimal PrecioConIva
        {
            get { return precioConIva; }
            set { precioConIva = value; }
        }
        private decimal precioMp;

        public decimal PrecioMp
        {
            get { return precioMp; }
            set { precioMp = value; }
        }
        private decimal precioMa;

        public decimal PrecioMa
        {
            get { return precioMa; }
            set { precioMa = value; }
        }
        private decimal precioUc;

        public decimal PrecioUc
        {
            get { return precioUc; }
            set { precioUc = value; }
        }
        private decimal precioSt;

        public decimal PrecioSt
        {
            get { return precioSt; }
            set { precioSt = value; }
        }
        private int codigoAlmacen;

        public int CodigoAlmacen
        {
            get { return codigoAlmacen; }
            set { codigoAlmacen = value; }
        }
        private string nombreAlmacen;

        public string NombreAlmacen
        {
            get { return nombreAlmacen; }
            set { nombreAlmacen = value; }
        }
        private decimal stock;

        public decimal Stock
        {
            get { return stock; }
            set { stock = value; }
        }
    }
}
