using MimeKit;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace ГеймерShop
{
    public class EmailService
    {
        public async Task SendEmailAsync(string toName,string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта ГеймерShop", "gamershopp@mail.ru"));
            emailMessage.To.Add(new MailboxAddress(toName, email));
            emailMessage.Subject = subject;
            TextPart textPart = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = message
            };
            emailMessage.Body = textPart;

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.mail.ru", 465, true);
                await client.AuthenticateAsync("gamershopp@mail.ru", "321654gs");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
