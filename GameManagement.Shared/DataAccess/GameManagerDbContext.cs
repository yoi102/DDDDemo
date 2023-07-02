using GameManagement.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManagement.Shared.DataAccess
{
    public class GameManagerDbContext : DbContext
    {

        public GameManagerDbContext(DbContextOptions<GameManagerDbContext> options) : base(options)
        {

        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ImageUrl> ImageUrls { get; set; }



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
                b.HasMany(e => e.ImageUrl)
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
