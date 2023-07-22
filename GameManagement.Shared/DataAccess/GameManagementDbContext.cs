using GameManagement.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Shared.DataAccess
{
    public class GameManagementDbContext : DbContext
    {
        public GameManagementDbContext(DbContextOptions<GameManagementDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<DisplayItem> DisplayItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Company>(b =>
            {
                b.HasMany(e => e.Games)
                .WithOne(e => e.Company)
                .HasForeignKey(e => e.CompanyId);
            });
            builder.Entity<Game>(b =>
            {
                b.HasMany(e => e.DisplayItems)
                .WithOne(e => e.Game)
                .HasForeignKey(e => e.GameId);
            });
            builder.Entity<Game>(b =>
            {
                b.HasMany(e => e.Tags)
                .WithMany(e => e.Games);
            });
        }
    }
}