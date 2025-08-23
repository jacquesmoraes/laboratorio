namespace API.Services
{
    public class EmailService ( IConfiguration config )
    {
        private readonly IConfiguration _config = config;

        public async Task SendEmailAsync ( string toEmail, string subject, string body )
        {
            var apiKey = _config["SendGrid:ApiKey"] ?? throw new InvalidOperationException("SendGrid:ApiKey is not configured");
            var fromEmail = _config["SendGrid:FromEmail"] ?? throw new InvalidOperationException("SendGrid:FromEmail is not configured");
            var fromName = _config["SendGrid:FromName"] ?? "Sistema Laboratório";

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, body);

            var response = await client.SendEmailAsync(msg);

            if ( !response.IsSuccessStatusCode )
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                throw new InvalidOperationException ( $"Failed to send email. Status: {response.StatusCode}, Body: {responseBody}" );
            }
        }
    }
}