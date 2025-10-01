using BankingSolution.Domain.Entities;
using BankingSolution.Domain.Indicators;
using FluentAssertions;

namespace BankingSolution.Tests.Unit;

public class AccountTests
{
    private readonly User _user = new("John", "Doe");

    [Fact]
    public void Create_WithNegativeInitialAmount_ShouldReturnError()
    {
        var result = Account.Create(_user, -10);

        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeOfType<InvalidOperationAmount>();
    }

    [Fact]
    public void Create_WithZeroInitialAmount_ShouldReturnAccountWithZeroBalance()
    {
        var result = Account.Create(_user, 0);

        result.IsT0.Should().BeTrue();
        result.AsT0.Balance.Should().Be(0);
    }

    [Fact]
    public void Create_WithPositiveInitialAmount_ShouldReturnAccountWithBalance()
    {
        var result = Account.Create(_user, 100);

        result.IsT0.Should().BeTrue();
        result.AsT0.Balance.Should().Be(100);
    }

    [Fact]
    public void Replenish_WithNegativeAmount_ShouldReturnError()
    {
        var account = Account.Create(_user, 0).AsT0;

        var result = account.Replenish(-5);

        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeOfType<InvalidOperationAmount>();
    }

    [Fact]
    public void Replenish_WithValidAmount_ShouldIncreaseBalance()
    {
        var account = Account.Create(_user, 0).AsT0;

        var result = account.Replenish(50);

        result.IsT0.Should().BeTrue();
        account.Balance.Should().Be(50);
    }

    [Fact]
    public void Replenish_WithSelfTransfer_ShouldReturnError()
    {
        var account = Account.Create(_user, 0).AsT0;

        var result = account.Replenish(10, account);

        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeOfType<SelfTransfer>();
    }

    [Fact]
    public void Withdraw_WithNegativeAmount_ShouldReturnError()
    {
        var account = Account.Create(_user, 100).AsT0;

        var result = account.Withdraw(-20);

        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeOfType<InvalidOperationAmount>();
    }

    [Fact]
    public void Withdraw_MoreThanBalance_ShouldReturnError()
    {
        var account = Account.Create(_user, 50).AsT0;

        var result = account.Withdraw(100);

        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeOfType<EnoughFunds>();
    }

    [Fact]
    public void Withdraw_WithValidAmount_ShouldDecreaseBalance()
    {
        var account = Account.Create(_user, 100).AsT0;

        var result = account.Withdraw(40);

        result.IsT0.Should().BeTrue();
        account.Balance.Should().Be(60);
    }

    [Fact]
    public void Withdraw_WithSelfTransfer_ShouldReturnError()
    {
        var account = Account.Create(_user, 100).AsT0;

        var result = account.Withdraw(20, account);

        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeOfType<SelfTransfer>();
    }
}