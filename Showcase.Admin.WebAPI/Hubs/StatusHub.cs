using Microsoft.AspNetCore.SignalR;

namespace Showcase.Admin.WebAPI.Hubs
{
    public class StatusHub : Hub
    {

        public Task SendPublicMessage(string message)
        {

            return Task.CompletedTask;
        }
        //.....

    }
}