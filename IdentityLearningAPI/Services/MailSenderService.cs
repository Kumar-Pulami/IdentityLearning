using System.Net;
using System.Net.Mail;
using IdentityLearningAPI.Configurations.MailConfig;
using IdentityLearningAPI.Interfaces;
using IdentityLearningAPI.Models;
using IdentityLearningAPI.Models.DTO;
using Microsoft.Extensions.Options;
using MimeKit;
using Microsoft.AspNetCore.Hosting;

namespace IdentityLearningAPI.Services
{
    public class MailSenderService : IMailSender
    {
        private readonly MailConfig _mailConfig;
        private string _bodyContent;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;

        public MailSenderService(IOptions<MailConfig> mailConfig, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _mailConfig = mailConfig.Value;
            _environment = environment;
        }

        public async Task SendAccountCreatedMail(User user)
        {
            Mail mailData = new Mail()
            {
                To = new List<string> { user.Email },
            };
            await SendCustomEmail(mailData);
        }

        public async Task SendConfirmEmailAddressMail(User user, string token)
        {
            Mail mailData = new Mail()
            {
                To = new List<string> { user.Email },
            };
            await SendCustomEmail(mailData);
        }

        public async Task SendForgotPasswordMail(User user, string token)
        {
            var pathToFile = _environment.WebRootPath
                                 + Path.DirectorySeparatorChar.ToString()
                                 + "EmailHTMLTemplete"
                                 + Path.DirectorySeparatorChar.ToString()
                                 + "ForgotPasswordTemplate.html";

            var builder = new BodyBuilder();
            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }

            _bodyContent = builder.HtmlBody.Replace("%invitationLink%", $"http://localhost:4200/SetPassword?email={user.Email}&token={token}");

            Mail mailData = new Mail()
            {
                To = new List<string> { user.Email },
                Subject = "Reset Your Password",
                Body = _bodyContent,
                Attachments = null
            };
            await SendCustomEmail(mailData);
        }

        public async Task SendCustomEmail(Mail mailData)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(_mailConfig.From, _mailConfig.DisplayName);
            mailMessage.Subject = mailData.Subject;
            mailMessage.Body = mailData.Body;
            mailMessage.IsBodyHtml = true;
            if (mailData.To.Count != 0)
            {
                foreach(string individualEmail in mailData.To){
                    mailMessage.To.Add(new MailAddress(individualEmail));
                }
            }

            if (mailData.Attachments != null)
            {
                Attachment attachedFile;
                Stream streamData = null;
                foreach(IFormFile attachment in mailData.Attachments)
                {
                    if (attachment.Length > 0)
                    {
                        attachment.CopyTo(streamData);
                        attachedFile = new Attachment(streamData, attachment.FileName);
                        mailMessage.Attachments.Add(attachedFile);
                    }                    
                }
            }

            SmtpClient smtpClient = new SmtpClient()
            {
                Host = _mailConfig.Host,
                Port = _mailConfig.Port,
                Credentials = new NetworkCredential(_mailConfig.From, _mailConfig.Password),
                EnableSsl = _mailConfig.UseSSL
            };
            await smtpClient.SendMailAsync(mailMessage);
            smtpClient.Dispose();
        }

        public async Task SendNewUserInvitationMail(string name, string emailAddress, string invaitationToken)
        {
            var pathToFile = _environment.WebRootPath
                                + Path.DirectorySeparatorChar.ToString()
                                + "EmailHTMLTemplete"
                                + Path.DirectorySeparatorChar.ToString()
                                + "InvitationTemplate.html";

            var builder = new BodyBuilder();
            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }

            var withName = builder.HtmlBody.Replace("%name%", $"{name}");
            builder.HtmlBody = withName;

            _bodyContent = builder.HtmlBody.Replace("%invitationLink%", $"http://localhost:4200/SignUp?email={emailAddress}&token={invaitationToken}");

            Mail mailData = new Mail()
            {
                To = new List<string> { emailAddress },
                Subject = "Account Creation Invitation",
                Body = _bodyContent,
                Attachments = null
            };
            await SendCustomEmail(mailData);
        }
    }
}