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
        
        public static MySqlConnection GetConnectionConta()
        {
            // leer la cadena de conexion del config
            var connectionString = ConfigurationManager.ConnectionStrings["Conta"].ConnectionString;
            // crear la conexion y devolverla.
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        
        #endregion 
        
        #region Usuario
        
        public static Usuario GetUsuario(MySqlDataReader rdr)
        {
            if (rdr.IsDBNull(rdr.GetOrdinal("CODUSU")))
                return null;
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
            
        public static Articulo GetArticuloEan(MySqlDataReader rdr)
        {
            Articulo a = new Articulo();
            a.CodigoArticulo = rdr.GetString("CODIGO_ARTICULO");
            a.CodigoEan = rdr.GetString("CODIGO_EAN");
            a.Status = rdr.GetInt32("STATUS");
            a.NombreArticulo = rdr.GetString("NOMBRE_ARTICULO");
            a.CodigoIva = rdr.GetInt32("CODIGO_IVA");
            a.PrecioSinIva = rdr.GetDecimal("PRECIO_SIN_IVA");
            a.PrecioMp = rdr.GetDecimal("PRECIOMP_ARTICULO");
            a.PrecioMa = rdr.GetDecimal("PRECIOMA_ARTICULO");
            a.PrecioUc = rdr.GetDecimal("PRECIOUC_ARTICULO");
            a.PrecioSt = rdr.GetDecimal("PRECIOST_ARTICULO");
            a.CodigoAlmacen = rdr.GetInt32("CODIGO_ALMACEN");
            a.NombreAlmacen = rdr.GetString("NOMBRE_ALMACEN");
            a.Stock = rdr.GetDecimal("STOCK");
            return a;
        }
            
        public static Articulo GetArticuloEan(string ean)
        {
            Articulo a = null;
            // primero obtenemos las lista
            using (MySqlConnection conn = GetConnectionAriges())
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                string sql = @"SELECT 
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
                        LEFT JOIN salmpr AS alp ON alp.codalmac = alm.codalmac
                        WHERE ar3.codigoea = '{0}'";
                sql = String.Format(sql, ean);
                cmd.CommandText = sql;
                MySqlDataReader rdr = cmd.ExecuteReader(); 
                if (rdr.HasRows)
                {
                    rdr.Read();
                    a = GetArticuloEan(rdr);
                    decimal porIva = GetPorIva(a.CodigoIva);
                    a.PrecioConIva = a.PrecioSinIva + ((a.PrecioSinIva * porIva) / 100M);
                }
            }
            return a;
        }
            
        public static IList<Articulo> GetArticulosEan(string ean)
        {
            IList<Articulo> la = new List<Articulo>();
            // primero obtenemos las lista
            using (MySqlConnection conn = GetConnectionAriges())
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                string sql = @"SELECT 
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
                        LEFT JOIN salmpr AS alp ON alp.codalmac = alm.codalmac
                        WHERE ar3.codigoea = '{0}'";
                sql = String.Format(sql, ean);
                cmd.CommandText = sql;
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        Articulo a = GetArticuloEan(rdr);
                        if (a != null)
                        {
                            // obtener el precio con iva
                            decimal porIva = GetPorIva(a.CodigoIva);
                            a.PrecioConIva = a.PrecioSinIva + ((a.PrecioSinIva * porIva) / 100M);
                            la.Add(a);
                        }
                    }
                }
            }
            return la;
        }
            
        public static decimal GetPorIva(int codigiva)
        {
            decimal p = 0;
            using (MySqlConnection conn = GetConnectionConta())
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                string sql = @"SELECT porceiva FROM tiposiva WHERE codigiva = {0}";
                sql = String.Format(sql, codigiva);
                cmd.CommandText = sql;
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    p = rdr.GetDecimal("porceiva");
                }
                rdr.Close();
                conn.Close();
            }
            return p;
        }
            
        public static void SetInventario(string codartic, int codalmac, decimal stock, decimal cantidad, decimal importe, int codigope)
        {
            MySqlConnection conn = null;
            MySqlTransaction tr = null;
                
            try
            {
                conn = GetConnectionAriges();
                conn.Open();
                // inicia transacción
                tr = conn.BeginTransaction();
                // procesos protegidos por la transacción
                SetShinve(codartic, codalmac, conn);
                SetSmoval(codartic, codalmac, stock, cantidad, importe, codigope, conn);
                SetSalmac(codartic, codalmac, cantidad, conn);
                // fin transacción.
                tr.Commit();
            }
            catch (MySqlException ex)
            {
                try
                {
                    tr.Rollback();
                    throw ex;
                }
                catch (MySqlException ex2)
                {
                    throw ex2;
                }
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        private static void SetShinve(string codartic, int codalmac, MySqlConnection conn)
        {
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"INSERT IGNORE INTO shinve(codartic, codalmac, fechainv, horainve, existenc)
                            SELECT codartic, codalmac, fechainv, horainve, stockinv FROM salmac
                            WHERE codartic = '{0}' AND codalmac = {1}";
            sql = String.Format(sql, codartic, codalmac);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        private static void SetSmoval(string codartic, int codalmac, decimal stock, decimal cantidad, decimal importe, int codigope, MySqlConnection conn)
        {
            decimal diferencia = cantidad - stock;
            int tipoMovi = 1;
            if (diferencia < 0)
            {
                cantidad = -cantidad;
                tipoMovi = 0;
            }
            //
            MySqlCommand cmd = conn.CreateCommand();
            //                                    0         1         2         3         4         5         6          7         8         9        10
            string sql = @"INSERT INTO smoval (codartic, codalmac, fechamov, horamovi, tipomovi, detamovi, impormov, codigope, letraser, document, numlinea, cantidad)
                    VALUES ('{0}',{1},'{2:yyyy-MM-dd}','{3:yyyy-MM-dd HH:mm:ss}',{4},'{5}',{6},'{7}','{8}','{9}',{10}, {11})";
            sql = String.Format(sql, codartic, codalmac, DateTime.Now, DateTime.Now, tipoMovi, "DFI", (diferencia * importe).ToString().Replace(',','.'), codigope, "", "LECTOR", 1, cantidad);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        private static void SetSalmac(string codartic, int codalmac, decimal cantidad, MySqlConnection conn)
        {
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"UPDATE salmac SET 
                            canstock = {0}, statusin=0, stockinv={0}, fechainv='{1:yyyy-MM-dd}', horainve='{1:yyyy-MM-dd HH:mm:ss}'
                            WHERE codartic = '{2}' AND codalmac = {3}";
            sql = String.Format(sql, cantidad, DateTime.Now, codartic, codalmac);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        #endregion 
    }
}