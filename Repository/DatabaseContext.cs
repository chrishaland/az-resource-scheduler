using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Repository;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Models.Environment> Environments => Set<Models.Environment>();
    public DbSet<Models.ResourceStopJob> ResourceStopJobs => Set<Models.ResourceStopJob>();

    public DbSet<Models.Provider> Providers => Set<Models.Provider>();
    public DbSet<Models.AzureProvider> AzureProviders => Set<Models.AzureProvider>();

    public DbSet<Models.Resource> Resources => Set<Models.Resource>();
    public DbSet<Models.VirtualMachine> VirtualMachines => Set<Models.VirtualMachine>();
    public DbSet<Models.VirtualMachineScaleSet> VirtualMachineScaleSets => Set<Models.VirtualMachineScaleSet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.Resource>()
            .ToTable("Resources")
            .HasIndex(r => new { r.Name, r.ResourceGroup })
            .IsUnique();

        modelBuilder.Entity<Models.Resource>()
            .HasMany(r => r.Environments)
            .WithMany(e => e.Resources)
            .UsingEntity(b => b.ToTable("EnvironmentResources"));

        BindResource(modelBuilder, r => r.VirtualMachine);
        BindResource(modelBuilder, r => r.VirtualMachineScaleSet);

        BindProvider(modelBuilder, r => r.AzureProvider);
    }

    private static void BindResource<T>(ModelBuilder modelBuilder, Expression<Func<Models.Resource, T?>> relationFunc) where T : Models.Resource
    {
        modelBuilder.Entity<T>(r =>
        {
            r.ToTable("Resources");
            r.Property(r => r.Name).HasColumnName("Name");
            r.Property(r => r.ResourceGroup).HasColumnName("ResourceGroup");
            r.Property(r => r.Description).HasColumnName("Description");
        });

        modelBuilder.Entity<Models.Resource>()
            .HasOne(relationFunc)
            .WithOne()
            .HasForeignKey<T>(r => r.Id);
    }

    private static void BindProvider<T>(ModelBuilder modelBuilder, Expression<Func<Models.Provider, T?>> relationFunc) where T : Models.Provider
    {
        modelBuilder.Entity<T>(r =>
        {
            r.ToTable("Providers");
            r.Property(r => r.Name).HasColumnName("Name");
        });

        modelBuilder.Entity<Models.Provider>()
            .HasOne(relationFunc)
            .WithOne()
            .HasForeignKey<T>(r => r.Id);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
        .UseLoggerFactory(LoggerFactory.Create(builder => { }));
}
