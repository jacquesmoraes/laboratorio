using System.Net;
using System.Net.Mail;

namespace API.Services
{
    public class EmailService ( IConfiguration config, ILogger<EmailService> logger )
    {
        private readonly IConfiguration _config = config;
        private readonly ILogger<EmailService> _logger = logger;

        public async Task SendEmailAsync ( string toEmail, string subject, string body )
        {
            var smtpSettings = _config.GetSection("SmtpSettings");

            var smtpServer = smtpSettings["Server"] ?? throw new InvalidOperationException("SMTP Server não configurado.");
            var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
            var smtpUsername = smtpSettings["Username"] ?? throw new InvalidOperationException("SMTP Username não configurado.");
            var smtpPassword = smtpSettings["Password"] ?? throw new InvalidOperationException("SMTP Password não configurado.");
            var fromEmail = smtpSettings["FromEmail"] ?? throw new InvalidOperationException("SMTP FromEmail não configurado.");
            var fromName = smtpSettings["FromName"] ?? "Sistema Laboratório";
            var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000 // 30 segundos timeout
            };

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add ( toEmail );

            try
            {
                _logger.LogInformation ( "Tentando enviar e-mail para {To} via {Server}:{Port} com SSL: {Ssl}",
                    toEmail, smtpServer, smtpPort, enableSsl );

                await client.SendMailAsync ( message );
                _logger.LogInformation ( "E-mail enviado com sucesso para {To}", toEmail );
            }
            catch ( SmtpException smtpEx )
            {
                _logger.LogError ( smtpEx, "Erro SMTP ao enviar e-mail para {To}. Status: {StatusCode}, Response: {Response}",
                    toEmail, smtpEx.StatusCode, smtpEx.StatusCode.ToString ( ) );
                throw new InvalidOperationException ( $"Erro SMTP: {smtpEx.StatusCode} - {smtpEx.Message}" );
            }
            catch ( Exception ex )
            {
                _logger.LogError ( ex, "Erro inesperado ao enviar e-mail para {To}", toEmail );
                throw new InvalidOperationException ( $"Erro ao enviar e-mail: {ex.Message}" );
            }
        }
    }
}
