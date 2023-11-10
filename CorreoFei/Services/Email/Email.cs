using CorreoFei.Services.ErrorLog;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace CorreoFei.Services.Email
{
    public class Email : IEmail
    {
        private readonly IErrorLog _errorLog;
        private readonly IConfiguration Configuracion;

        public Email(IErrorLog errorLog, IConfiguration configuracion)
        {
            _errorLog = errorLog;
            Configuracion = configuracion;
        }

        [HttpPost]
        public Task<bool> EnviarCorreoAsync(string tema, string para, string cc, string bcc, string cuerpo, Attachment adjunto = null)
        {
            bool res = false;
            try
            {
                //Rutina para mandar el mail
                MailMessage eMail = new();

                if(para != null)
                    eMail.To.Add(para);
                if (cc != null)
                    eMail.CC.Add(cc);
                if (bcc != null)
                    eMail.Bcc.Add(bcc);

                eMail.From = new MailAddress(Configuracion["smtp:SmtpUser"]);
                if (string.IsNullOrEmpty(tema))
                    tema = "[sin asunto]";
                eMail.Subject = tema;
                eMail.Body = cuerpo;
                if (adjunto != null)
                    eMail.Attachments.Add(adjunto);
                eMail.BodyEncoding = System.Text.Encoding.UTF8;
                eMail.SubjectEncoding = System.Text.Encoding.UTF8;
                eMail.HeadersEncoding = System.Text.Encoding.UTF8;
                eMail.IsBodyHtml = true;

                SmtpClient clienteSMTP = new(Configuracion["Smtp:SmtpServer"]);
                clienteSMTP.Port = Convert.ToInt16(Configuracion["Smtp:SmtpPort"]);
                clienteSMTP.EnableSsl = true;
                clienteSMTP.DeliveryMethod = SmtpDeliveryMethod.Network;
                clienteSMTP.UseDefaultCredentials = false;
                clienteSMTP.Credentials = new System.Net.NetworkCredential(Configuracion["Smtp:SmtpUser"], Configuracion["Smtp:SmtpPwd"]);

                //Envia el correo
                _errorLog.ErrorLogAsync($"Correo para {para} con tema {tema}");
                clienteSMTP.SendAsync(eMail, null);
                res = true;
                
            }
            catch(SmtpException ex)
            {
                _errorLog.ErrorLogAsync(ex.Message);
            }
            return Task.FromResult(res);
        }
    }
}
