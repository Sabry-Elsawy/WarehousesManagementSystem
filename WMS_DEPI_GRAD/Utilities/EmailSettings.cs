using System.Net;
using System.Net.Mail;

namespace WMS_DEPI_GRAD.Utilities
{
    public static class EmailSettings
    {
        public static void SendEmail(Email email)
        {
            // Sending Email
            var client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("elsawysabry16@gmail.com", "efyy crgbwylepgza");
            client.Send("elsawysabry16@gmail.com", email.To, email.Subject, email.Body);
        }
    }
}
