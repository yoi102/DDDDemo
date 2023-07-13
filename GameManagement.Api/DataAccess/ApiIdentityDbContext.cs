using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Api.DataAccess
{
    public class ApiIdentityDbContext : IdentityDbContext<ApiIdentityUser,ApiIdentityRole,long>
    {
        public ApiIdentityDbContext(DbContextOptions<ApiIdentityDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}