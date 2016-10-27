using ConsoleApplication1.Database;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace Rechnungsversand
{
    public static class dbconnect
    {
        private static string myConnectionString;
        public static MySqlConnection connection;
        public static async Task updaterecord(string rechnungsnr, bool status)
        {
            MySqlCommand command = new MySqlCommand();
            string ts = DateTime.Now.ToString();
            string SQL = string.Format("INSERT INTO `rechnungsversand` (`rechnungsnr`, `status`, 'zeit' ) VALUES ('{0}', '{1}', '{2}')", rechnungsnr, status,ts);
            command.CommandText = SQL;
            
            command.Connection = connection;
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }

        public static void OpenConnection()
        {
            try
            {
                myConnectionString = "Server = 91.89.177.228; DATABASE = lexwarenew; UID = rechnungsversand; PASSWORD = iifywXeEiNuInopl";
                connection = new MySqlConnection(myConnectionString);
                connection.Open();
                Console.WriteLine("MySQL Connection established.");
            }
            catch(Exception ex) { Console.WriteLine(ex.Message); }
        }

        public static void CloseConnection()
        {
            try
            {
                if(connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
	}
}
