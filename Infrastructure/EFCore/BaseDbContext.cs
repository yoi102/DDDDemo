using DomainCommons;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EFCore
{
    public abstract class BaseDbContext : DbContext
    {
        private readonly IMediator mediator;

        public BaseDbContext(DbContextOptions options, IMediator mediator) : base(options)
        {
            this.mediator = mediator;
        }

        [Obsolete("Don not call SaveChanges, please call SaveChangesAsync instead.")]
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new NotImplementedException("Don not call SaveChanges, please call SaveChangesAsync instead.");
        }

        //另一个重载的SaveChangesAsync(CancellationToken cancellationToken)也调用这个SaveChangesAsync方法。所以在这里统一控制
        public async override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {


            await mediator.DispatchDomainEventsAsync(this);

            var softDeletedEntities = this.ChangeTracker.Entries<ISoftDelete>()
                 .Where(e => e.State == EntityState.Modified && e.Entity.IsDeleted)
                 .Select(e => e.Entity)
                 .ToList();//防止延迟加载

            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            //把被软删除的对象从 Cache 删除
            softDeletedEntities.ForEach(e => this.Entry(e).State = EntityState.Detached);

            return result;
        }
    }
}