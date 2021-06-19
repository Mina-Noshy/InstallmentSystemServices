using Microsoft.AspNetCore.Hosting;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MyWebModels.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyWebModels.Sittings;

namespace MyWebAPI.Services.Mailing
{
    public interface IMailServices
    {
        Task<bool> SendEmail(string fromName,
                            string fromAddress,
                            string toName,
                            string toAddress,
                            string subject,
                            string message,
                            bool IsplainText);
    }

    public class MailServices : IMailServices
    {
        private readonly MailConfiguration mailConfig;
        private readonly IHostingEnvironment hosting;

        public MailServices(MailConfiguration _mailConfig, IHostingEnvironment _hosting)
        {
            mailConfig = _mailConfig;
            hosting = _hosting;
        }
        public async Task<bool> SendEmail(string fromName,
                                          string fromAddress,
                                          string toName,
                                          string toAddress,
                                          string subject,
                                          string message,
                                          bool IsplainText)
        {
            try
            {
                if (string.IsNullOrEmpty(fromName))
                    fromName = mailConfig.UserName;

                if (string.IsNullOrEmpty(fromAddress))
                    fromAddress = mailConfig.EmailAddress;

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(fromName, fromAddress));
                mimeMessage.To.Add(new MailboxAddress(toName, toAddress));
                mimeMessage.Subject = subject;

                if(IsplainText)
                {
                    mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                    {
                        Text = message
                    };
                }
                else
                {
                    mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                    {
                        Text = message
                    };
                }


                return await Send(mimeMessage);

            }
            catch
            {
                return await Task.FromResult<bool>(false);
            }
        }

        private async Task<bool> Send(MimeMessage mimeMessage)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(mailConfig.SmtpServer, mailConfig.Port, SecureSocketOptions.StartTls);
                    client.Authenticate(mailConfig.EmailAddress, mailConfig.Password);
                    client.Send(mimeMessage);
                    client.Disconnect(true);
                }

                return await Task.FromResult<bool>(true);
            }
            catch
            {
                return await Task.FromResult<bool>(false);
            }
        }

    }
}
