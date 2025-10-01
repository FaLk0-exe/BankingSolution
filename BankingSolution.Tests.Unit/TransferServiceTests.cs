using BankingSolution.Domain.Entities;
using BankingSolution.Domain.Indicators;
using BankingSolution.Domain.Services;
using FluentAssertions;

namespace BankingSolution.Tests.Unit;

public class TransferServiceTests
{
    private readonly User _user1 = new("Alice", "Smith");
    private readonly User _user2 = new("Bob", "Johnson");
    private readonly TransferService _service = new();

    [Fact]
    public void TransferFunds_WithEnoughBalance_ShouldSucceed()
    {
        var accFrom = Account.Create(_user1, 100).AsT0;
        var accTo = Account.Create(_user2, 0).AsT0;

        var result = _service.TransferFunds(accTo, accFrom, 50);

        result.IsT0.Should().BeTrue();
        accFrom.Balance.Should().Be(50);
        accTo.Balance.Should().Be(50);
    }

    [Fact]
    public void TransferFunds_NotEnoughFunds_ShouldReturnError()
    {
        var accFrom = Account.Create(_user1, 20).AsT0;
        var accTo = Account.Create(_user2, 0).AsT0;

        var result = _service.TransferFunds(accTo, accFrom, 100);

        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeOfType<EnoughFunds>();
    }

    [Fact]
    public void TransferFunds_SelfTransfer_ShouldReturnError()
    {
        var acc = Account.Create(_user1, 100).AsT0;

        var result = _service.TransferFunds(acc, acc, 20);

        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeOfType<SelfTransfer>();
    }
}