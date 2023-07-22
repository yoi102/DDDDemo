using DomainCommons;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure
{
    public static class MediatorExtensions
    {
        public static IServiceCollection AddMediatR(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            return services.AddMediatR(assemblies.ToArray());
        }

        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<IDomainEvents>()
                .Where(x => x.Entity.GetDomainEvents().Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.GetDomainEvents())
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}