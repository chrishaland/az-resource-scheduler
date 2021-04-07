using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Models;
using System;
using System.Linq.Expressions;
using Environment = Repository.Models.Environment;

namespace Repository
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Environment> Environments { get; set; }

        public DbSet<Resource> Resources { get; set; }
        public DbSet<VirtualMachine> VirtualMachines { get; set; }
        public DbSet<VirtualMachineScaleSet> VirtualMachineScaleSets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>()
                .ToTable("Resources")
                .HasIndex(r => new { r.Name, r.ResourceGroup })
                .IsUnique();

            modelBuilder.Entity<Resource>()
                .HasMany(r => r.Environments)
                .WithMany(e => e.Resources)
                .UsingEntity(b => b.ToTable("EnvironmentResources"));

            BindResource(modelBuilder, r => r.VirtualMachine);
            BindResource(modelBuilder, r => r.VirtualMachineScaleSet);
        }

        private void BindResource<T>(ModelBuilder modelBuilder, Expression<Func<Resource, T>> relationFunc) where T : Resource
        {
            modelBuilder.Entity<T>(r =>
            {
                r.ToTable("Resources");
                r.Property(r => r.Name).HasColumnName("Name");
                r.Property(r => r.ResourceGroup).HasColumnName("ResourceGroup");
                r.Property(r => r.Description).HasColumnName("Description");
            });

            modelBuilder.Entity<Resource>()
                .HasOne(relationFunc)
                .WithOne()
                .HasForeignKey<T>(r => r.Id);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
            .UseLoggerFactory(LoggerFactory.Create(builder => { }));
    }
}
