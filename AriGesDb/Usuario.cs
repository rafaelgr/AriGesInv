using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriGesDb
{
    public class Usuario
    {
        private int codUsu;

        public int CodUsu
        {
            get { return codUsu; }
            set { codUsu = value; }
        }
        private string nomUsu;

        public string NomUsu
        {
            get { return nomUsu; }
            set { nomUsu = value; }
        }
        private string passwordPropio;

        public string PasswordPropio
        {
            get { return passwordPropio; }
            set { passwordPropio = value; }
        }
        private string login;

        public string Login
        {
            get { return login; }
            set { login = value; }
        }

        private int codtraba;

        public int Codtraba
        {
            get { return codtraba; }
            set { codtraba = value; }
        }

    }
}
