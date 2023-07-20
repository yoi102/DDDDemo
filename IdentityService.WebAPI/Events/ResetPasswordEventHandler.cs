using IdentityService.Domain;
using Microsoft.Extensions.Logging;
using Zack.EventBus;

namespace IdentityService.WebAPI.Events
{
    [EventName("IdentityService.User.PasswordReset")]
    public class ResetPasswordEventHandler : JsonIntegrationEventHandler<ResetPasswordEvent>
    {
        private readonly ISmsSender smsSender;

        public ResetPasswordEventHandler(ISmsSender smsSender)
        {
            this.smsSender = smsSender;
        }

        public override Task HandleJson(string eventName, ResetPasswordEvent? eventData)
        {
            if (eventData is not null)
            {
                //发送密码给被用户的手机
                return smsSender.SendAsync(eventData.PhoneNum, eventData.Password);
            }
            return Task.CompletedTask;
        }
    }
}