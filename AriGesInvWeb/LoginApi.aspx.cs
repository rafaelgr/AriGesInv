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
    public partial class LoginApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #region WebMethods
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Usuario GetLogin(string login, string password)
        {
            return CntAriGesDb.GetUsuario(login, password);
        }
        #endregion 
    }
}