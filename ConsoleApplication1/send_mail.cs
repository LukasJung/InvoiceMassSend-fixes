using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Rechnungsversand
{
    public class send_mail
    {
        public static async Task send(string adresse, string betreff, string anhang, string rnr)
        {
            using (MailMessage wunschreich_mail = new MailMessage())
            using (SmtpClient client = new SmtpClient())
            using (Attachment attachment = new System.Net.Mail.Attachment(anhang))
            {
                client.Port = 25;
                client.Host = "mail.1und1.de";
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("service@wunschreich.de", "test");
                client.EnableSsl = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                wunschreich_mail.IsBodyHtml = true;
                wunschreich_mail.AlternateViews.Add(getEmbeddedImage(Directory.GetCurrentDirectory()+ "\\signatur.png"));
                wunschreich_mail.From = new MailAddress("service@wunschreich.de");
                wunschreich_mail.To.Add(new MailAddress(adresse));
                wunschreich_mail.Subject = betreff;
                wunschreich_mail.Attachments.Add(attachment);


                int retry = 0;
                while (retry < 3)
                    try
                    {
                        await client.SendMailAsync(wunschreich_mail);
                        await dbconnect.updaterecord(rnr, true);
                        attachment.Dispose();
                        break;
                    }
                    catch
                    {
                        var err = dbconnect.updaterecord(rnr, false);
                        attachment.Dispose();
                        retry++;
                    }
            }
        }


        private static AlternateView getEmbeddedImage(string filePath)
        {
            LinkedResource inline = new LinkedResource(filePath);
            inline.ContentId = Guid.NewGuid().ToString();
            string htmlBody = string.Format(@"
            <p>Sehr geehrter Kunde,</p>

            <p>im Anhang erhalten sie die Rechnung zu ihrer Bestellung.</p>

            <p>Mit freundlichen Grüßen</p>
            <br />

            <p>Wunschreich Kundenservice</p>
           <img src=""cid:{0}"" />
            <p>
            WUNSCHREICH GmbH<br />
            Agentur für Inneneinrichtung<br />
            Ostring 8<br />
            D-76131 Karlsruhe</p>

            <p>Geschäftsführung: Marc Breig, Jochen Jagla<br />
            Steuer-Nr. DE269622660</p>

            <p>Telefon: 0721 / 4705179 - 6<br />
            Fax: 0721 / 4705179 - 9</p>

            <p> Email: <a href='mailto:info@wunschreich.de'>info@wunschreich.de</a><br />
            <a href='http://www.wunschreich.de'>http://www.wunschreich.de</a></p>

            <p>
            </p>
            ", inline.ContentId);
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(inline);
            return alternateView;
        }
    }
}