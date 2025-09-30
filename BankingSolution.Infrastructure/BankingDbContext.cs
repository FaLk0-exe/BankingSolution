using BankingSolution.Domain.Entities;
using BankingSolution.Infrastructure.Converters;
using BankingSolution.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace BankingSolution.Infrastructure;

public class BankingDbContext:DbContext
{
    public DbSet<User> Users { get; init; }
    public DbSet<Account> Accounts { get; init; }
    
    public BankingDbContext(DbContextOptions<BankingDbContext> options)
        : base(options)
    {
    }
    
    public BankingDbContext()
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<Ulid>()
            .HaveConversion<UlidConverter>();
        configurationBuilder.Properties<Ulid?>()
            .HaveConversion<NullableUlidConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new AccountTransactionConfiguration());
        modelBuilder.ApplyConfiguration(new ReplenishAccountTransactionConfiguration());
        modelBuilder.ApplyConfiguration(new WithdrawAccountTransactionConfiguration());
    }
}