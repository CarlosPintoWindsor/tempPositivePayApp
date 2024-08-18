using SendGrid.Helpers.Mail;
using SendGrid;

namespace positive_pay_app.app
{
    internal class EmailService(
        AppConfig appConfig
        )
    {

        public class EmailPayload
        {
            public String Subject { get; set; }
            public string[] To { get; set; }
            public string? PlainTextContent { get; set; }
            public string? HtmlContent { get; set; }
        }

        public async Task SendEmail(EmailPayload EmailPayload)
        {
            var client = new SendGridClient(appConfig.EmailerConfig.SendGridApiKey);
            var from_email = new EmailAddress(appConfig.EmailerConfig.From);
            var subject = $"{appConfig.EmailerConfig.SubjectPrefix} - {EmailPayload.Subject}";
            var plainTextContent = EmailPayload.PlainTextContent;
            var htmlContent = EmailPayload.HtmlContent;
            var to_emails = EmailPayload.To.ToList().Select( to => new EmailAddress(to) ).ToList();
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from_email, to_emails, subject, plainTextContent, htmlContent);

            await client.SendEmailAsync(msg).ConfigureAwait(false);

        }



    }
}
