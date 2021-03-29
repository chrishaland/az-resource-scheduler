using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Models;

namespace Repository
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Environment> Environments { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>()
                .HasMany(r => r.Environments)
                .WithMany(e => e.Resources)
                .UsingEntity(b => b.ToTable("EnvironmentResources"));

            modelBuilder.Entity<Resource>()
                .HasMany(r => r.Tenants)
                .WithMany(t => t.Resources)
                .UsingEntity(b => b.ToTable("TenantResources"));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
            .UseLoggerFactory(LoggerFactory.Create(builder => { }));
    }
}
