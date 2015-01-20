using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using AriGesDb;

namespace AriGesInvWeb
{
    public partial class InventarioApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #region WebMethods
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Articulo GetArticuloEan(string ean)
        {
            return CntAriGesDb.GetArticuloEan(ean);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Articulo> GetArticulosEan(string ean)
        {
            return CntAriGesDb.GetArticulosEan(ean);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SetInventario(string codartic, int codalmac, decimal stock, decimal cantidad, decimal importe, int codigope)
        {
            string m = "*";
            try
            {
                CntAriGesDb.SetInventario(codartic, codalmac, stock, cantidad, importe, codigope);
            }
            catch (Exception ex)
            {
                m = ex.Message;
            }
            return m;
        }

        
        #endregion 
    }
}