using MediatR;
using Showcase.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Showcase.Domain.Events
{
    public record GameCreatedEvent(Game Game) : INotification;

}
