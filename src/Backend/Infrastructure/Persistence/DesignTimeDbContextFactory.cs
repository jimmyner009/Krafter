using Backend.Infrastructure.BackgroundJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Backend.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
    {
        public TenantDbContext CreateDbContext(string[] args)
        {
            // Prefer environment variable used elsewhere; fall back to local default
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__krafterDb")
                                   ??
                                   "Host=localhost;Port=56187;Username=postgres;Password=postgres;Database=krafterDb";
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure());

            return new TenantDbContext(optionsBuilder.Options);
        }
    }

    public class DesignTimeKrafterContextDbContextFactory : IDesignTimeDbContextFactory<KrafterContext>
    {
        public KrafterContext CreateDbContext(string[] args)
        {
            // Prefer environment variable used elsewhere; fall back to local default
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__krafterDb")
                                   ??
                                   "Host=localhost;Port=56187;Username=postgres;Password=postgres;Database=krafterDb";
            var optionsBuilder = new DbContextOptionsBuilder<KrafterContext>();
            optionsBuilder.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure());

            return new KrafterContext(optionsBuilder.Options,null,null);
        }
    }

    public class DesignTimeBackgroundJobsContextDbContextFactory : IDesignTimeDbContextFactory<BackgroundJobsContext>
    {
        public BackgroundJobsContext CreateDbContext(string[] args)
        {
            // Prefer environment variable used elsewhere; fall back to local default
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__krafterDb")
                                   ??
                                   "Host=localhost;Port=56187;Username=postgres;Password=postgres;Database=krafterDb";
            var optionsBuilder = new DbContextOptionsBuilder<BackgroundJobsContext>();
            optionsBuilder.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure());

            return new BackgroundJobsContext(optionsBuilder.Options);
        }
    }
}
