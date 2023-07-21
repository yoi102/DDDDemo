using Commons;
using IdentityService.Domain;
using MediatR;
using Zack.EventBus;

namespace IdentityService.WebAPI.Events
{

    public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
    {
        private readonly ISmsSender smsSender;

        public UserCreatedEventHandler(ISmsSender smsSender)
        {
            this.smsSender = smsSender;
        }
        public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            return smsSender.SendAsync(notification.PhoneNumber, notification.Password);
        }
    }
}
