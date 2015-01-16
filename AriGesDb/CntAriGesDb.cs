using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AriGesDb
{
    public static partial class CntAriGesDb
    {
        #region Conexiones
        public static MySqlConnection GetConnectionUsuarios()
        {
            // leer la cadena de conexion del config
            var connectionString = ConfigurationManager.ConnectionStrings["Usuarios"].ConnectionString;
            // crear la conexion y devolverla.
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        public static MySqlConnection GetConnectionAriges()
        {
            // leer la cadena de conexion del config
            var connectionString = ConfigurationManager.ConnectionStrings["Ariges"].ConnectionString;
            // crear la conexion y devolverla.
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        #endregion 

        #region Usuario
        public static Usuario GetUsuario(MySqlDataReader rdr)
        {
            if (rdr.IsDBNull(rdr.GetOrdinal("CODUSU"))) return null;
            Usuario u = new Usuario();
            u.CodUsu = rdr.GetInt32("CODUSU");
            u.NomUsu = rdr.GetString("NOMUSU");
            u.Login = rdr.GetString("LOGIN");
            u.PasswordPropio = rdr.GetString("PASSWORD_PROPIO");
            return u;
        }

        public static Usuario GetUsuario(string login, string password)
        {
            Usuario u = null;
            using (MySqlConnection conn = GetConnectionUsuarios())
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                string sql = @"SELECT 
                    codusu AS CODUSU, 
                    nomusu AS NOMUSU, 
                    login AS LOGIN, 
                    passwordpropio AS PASSWORD_PROPIO
                    FROM usuarios
                    WHERE login = '{0}'
                    AND passwordpropio = '{1}'";
                sql = String.Format(sql, login, password);
                cmd.CommandText = sql;
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    u = GetUsuario(rdr);
                }
                rdr.Close();

                if (u != null)
                {
                    using (MySqlConnection conn2 = GetConnectionAriges())
                    {
                        conn2.Open();
                        // hay que comprobar si tiene un registro
                        // en la tabla straba
                        sql = @"SELECT * FROM straba WHERE login = '{0}'";
                        sql = String.Format(sql, u.Login);
                        MySqlCommand cmd2 = conn2.CreateCommand();
                        cmd2.CommandText = sql;
                        rdr = cmd2.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            u.Codtraba = rdr.GetInt32("codtraba");
                        }
                        else
                        {
                            // un usuario sin trabajador usuario no se 
                            // puede usar.
                            u = null;
                        }
                        conn2.Close();
                    }
                }
                conn.Close();
            }
            return u;
        }
        #endregion

        #region Artículos
        
        #endregion 
    }
}
