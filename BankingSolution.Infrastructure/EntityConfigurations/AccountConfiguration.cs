using BankingSolution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSolution.Infrastructure.EntityConfigurations;

public class AccountConfiguration: IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property<Ulid>("UserId").IsRequired();
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Accounts)
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(s => s.AccountTransactions).AutoInclude();

    }
}