﻿using IdentityService.Domain;
using Zack.EventBus;

namespace IdentityService.WebAPI.Events
{
    [EventName("IdentityService.User.Created")]
    public class UserCreatedEventHandler : JsonIntegrationEventHandler<UserCreatedEvent>
    {
        private readonly ISmsSender smsSender;

        public UserCreatedEventHandler(ISmsSender smsSender)
        {
            this.smsSender = smsSender;
        }

        public override Task HandleJson(string eventName, UserCreatedEvent? eventData)
        {
            if (eventData is not null)
            {
                //发送初始密码给被创建用户的手机
                return smsSender.SendAsync(eventData.PhoneNum, eventData.Password);
            }
            return Task.CompletedTask;
        }



    }
}