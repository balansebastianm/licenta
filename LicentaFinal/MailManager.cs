using MimeKit;
namespace LicWeb
{
    public class MailManager
    {
        private string EmailAddress, Title, TextBody, SenderName, SenderSurname, ReceiverName, ReceiverSurname, RegistrationCode;
        public void SendMail(string EmailAddress, string Title, string TextBody, string RegistrationCode, string SenderName, string SenderSurname, string ReceiverName, string ReceiverSurname)
        {
            this.EmailAddress = EmailAddress;
            this.Title = Title;
            this.TextBody = TextBody;
            this.SenderName = SenderName;
            this.SenderSurname = SenderSurname;
            this.RegistrationCode = RegistrationCode;
            this.ReceiverSurname = ReceiverSurname;
            this.ReceiverName = ReceiverName;
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(SenderName + " " + SenderSurname, "Administrator"));
            email.To.Add(new MailboxAddress(ReceiverName + " " + ReceiverSurname, EmailAddress));

            email.Subject = Title;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = TextBody + RegistrationCode
            };
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    smtp.Connect("smtp.gmail.com", 587, false);
                    smtp.Authenticate("lucrarelicenta2023@gmail.com", "lujengmcgxyytdtd");
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public void SendMail(string EmailAddress, string Title, string TextBody, string SenderName, string SenderSurname, string ReceiverName, string ReceiverSurname)
        {
            this.EmailAddress = EmailAddress;
            this.Title = Title;
            this.TextBody = TextBody;
            this.SenderName = SenderName;
            this.SenderSurname = SenderSurname;
            this.ReceiverSurname = ReceiverSurname;
            this.ReceiverName = ReceiverName;
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(SenderName + " " + SenderSurname, "Administrator"));
            email.To.Add(new MailboxAddress(ReceiverName + " " + ReceiverSurname, EmailAddress));

            email.Subject = Title;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = TextBody + RegistrationCode
            };
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    smtp.Connect("smtp.gmail.com", 587, false);
                    smtp.Authenticate("lucrarelicenta2023@gmail.com", "lujengmcgxyytdtd");
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public void SendMail(string EmailAddress, string Title, string TextBody, string SenderName, string SenderSurname, string ReceiverName, string ReceiverSurname, string PrivateKeyPath, string PublicKeyPath, string CertificatePath)
        {
            this.EmailAddress = EmailAddress;
            this.Title = Title;
            this.TextBody = TextBody;
            this.SenderName = SenderName;
            this.SenderSurname = SenderSurname;
            this.ReceiverSurname = ReceiverSurname;
            this.ReceiverName = ReceiverName;

            var email = new MimeMessage();
            var builder = new BodyBuilder();
            builder.TextBody= TextBody;
            builder.Attachments.Add(PrivateKeyPath);
            builder.Attachments.Add(PublicKeyPath);
            builder.Attachments.Add(CertificatePath);
            email.From.Add(new MailboxAddress(SenderName + " " + SenderSurname, "Administrator"));
            email.To.Add(new MailboxAddress(ReceiverName + " " + ReceiverSurname, EmailAddress));

            email.Subject = Title;
            email.Body = builder.ToMessageBody();
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    smtp.Connect("smtp.gmail.com", 587, false);
                    smtp.Authenticate("lucrarelicenta2023@gmail.com", "lujengmcgxyytdtd");
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
