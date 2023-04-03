using IdentityLearningAPI.Models;
using IdentityLearningAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace IdentityLearningAPI.Interfaces
{
    public interface IMailSender
    {

        Task SendNewUserInvitationMail(string name, String emailAddress, String invaitationToken);
        Task SendAccountCreatedMail (User user);
        Task SendForgotPasswordMail(User user, String token);
        Task SendConfirmEmailAddressMail (User user, string token);
        Task SendCustomEmail (Mail mailData);
    }
}
