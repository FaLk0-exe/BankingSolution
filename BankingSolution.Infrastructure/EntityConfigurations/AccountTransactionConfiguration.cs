using BankingSolution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSolution.Infrastructure.EntityConfigurations;

public class AccountTransactionConfiguration:IEntityTypeConfiguration<AccountTransaction>
{
    public void Configure(EntityTypeBuilder<AccountTransaction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property<Ulid>("AccountId").IsRequired();

        builder.HasOne(x => x.Account)
            .WithMany(x => x.AccountTransactions)
            .HasForeignKey("AccountId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(s => s.Type).HasConversion<string>();
    }
}

public class ReplenishAccountTransactionConfiguration:IEntityTypeConfiguration<ReplenishAccountTransaction>
{
    public void Configure(EntityTypeBuilder<ReplenishAccountTransaction> builder)
    {
        builder.Property<Ulid>("AccountFromId").IsRequired();

        builder.HasOne(x => x.AccountFrom)
            .WithMany()
            .HasForeignKey("AccountFromId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class WithdrawAccountTransactionConfiguration:IEntityTypeConfiguration<WithdrawAccountTransaction>
{
    public void Configure(EntityTypeBuilder<WithdrawAccountTransaction> builder)
    {
        builder.Property<Ulid>("AccountToId").IsRequired();

        builder.HasOne(x => x.AccountTo)
            .WithMany()
            .HasForeignKey("AccountToId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}