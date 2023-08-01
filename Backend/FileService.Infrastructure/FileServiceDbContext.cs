using FileService.Domain.Entities;
using Infrastructure.EFCore;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FileService.Infrastructure
{
    public class FileServiceDbContext : BaseDbContext
    {
        public DbSet<UploadedItem> UploadItems { get; private set; }

        public FileServiceDbContext(DbContextOptions<FileServiceDbContext> options, IMediator mediator)
            : base(options, mediator)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}