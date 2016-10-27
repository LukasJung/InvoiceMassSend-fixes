using MySql.Data.MySqlClient;
using Rechnungsversand;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class Invoice
    {
        public static List<Invoice> GetInvoices { get { return Directory.GetFiles(Directory.GetCurrentDirectory(), "*.pdf").Select(path => Path.GetFileName(path)).Select(o => new Invoice(o)).ToList(); } }

        public string Name { get { return filename.Replace(".pdf", ""); } }

        private string filename;

        public Invoice(string f)
        {
            filename = f;
        }
        public async Task Send()
        {
            try
            {
                MySqlCommand command = dbconnect.connection.CreateCommand();
                command.CommandText = string.Format("SELECT `SheetNr`,`AuftragsNr`,`AuftragsKennung`,`VorgangNr`,`Anschrift_Email` FROM `fk_auftrag` WHERE `AuftragsNr` = '{0}' AND `AuftragsKennung` = 3;", filename.Replace(".pdf", ""));
                var reader = await command.ExecuteReaderAsync();
                var rnr = "";
                var adresse = "";
                var betreff = "";
                while (reader.Read())
                {
                    rnr = reader.GetString(1);
                    adresse = reader.GetString(4);
                    betreff = "Ihre Rechnung mit der Rechnungsnr: " + rnr + " von Wunschreich";
                }
                    reader.Close();
                string attach = Directory.GetCurrentDirectory()+"\\" + filename;
                await send_mail.send(adresse, betreff, attach, rnr);
                Console.WriteLine(attach);
                string destination = Directory.GetCurrentDirectory()+ "\\processed\\"+ rnr+ ".pdf";
                Console.WriteLine(destination);
                Console.WriteLine(DateTime.Now);
                File.Move(attach, destination);
                
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}
