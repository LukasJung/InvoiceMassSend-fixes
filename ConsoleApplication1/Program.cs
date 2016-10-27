using MySql.Data.MySqlClient;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;
using ConsoleApplication1;
using System;

namespace Rechnungsversand
{
    class Program
    {
        static void Main(string[] args)
        {
            dbconnect.OpenConnection(); // Open database connection globally; once for every action.
            MainAsync().Wait();
            dbconnect.CloseConnection();
        }

        private static async Task MainAsync()
        {
            var invoices = Invoice.GetInvoices;
            while (true)
            {
                if (invoices.Count() > 2)
                {
                    var first = invoices[0].Send();
                    var second = invoices[1].Send();
                    await Task.WhenAll(first, second);
                    invoices.RemoveAt(0);
                    invoices.RemoveAt(1);
                }
                else if (invoices.Count() == 1)
                {
                    await invoices[0].Send();
                    invoices.RemoveAt(0);
                }
                else break;
                invoices.RemoveAll(o => o == null);
                Console.WriteLine("Press any key to stop...");
                Console.ReadKey();
            }
        }
    }
}
