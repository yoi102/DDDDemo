using IdentityService.Domain;
using MediatR;


namespace IdentityService.WebAPI.Events
{



    public class ResetPasswordEventHandler : INotificationHandler<ResetPasswordEvent>
    {
        private readonly ISmsSender smsSender;

        public ResetPasswordEventHandler(ISmsSender smsSender)
        {
            this.smsSender = smsSender;
        }
        public Task Handle(ResetPasswordEvent notification, CancellationToken cancellationToken)
        {
            return smsSender.SendAsync(notification.PhoneNumber, notification.Password);
        }
    }





}