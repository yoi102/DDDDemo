using Infrastructure.EFCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Showcase.Domain.Entities;

namespace Showcase.Infrastructure
{
    public class ShowcaseDbContext : BaseDbContext
    {
        public DbSet<Company> Companies { get; private set; }
        public DbSet<Game> Games { get; private set; }
        public DbSet<Exhibit> Exhibits { get; private set; }
        public DbSet<Tag> Tags { get; private set; }


        public ShowcaseDbContext(DbContextOptions<ShowcaseDbContext> options, IMediator mediator) : base(options, mediator)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            modelBuilder.EnableSoftDeletionGlobalFilter();



        }
    }



}

