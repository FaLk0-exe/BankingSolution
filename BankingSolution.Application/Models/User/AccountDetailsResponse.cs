using BankingSolution.Domain.Enums;

namespace BankingSolution.Application.Models.User;

public record AccountResponse(
    Ulid Id,
    Ulid UserId,
    decimal Balance)
{
    public DateTimeOffset CreatedAt => Id.Time;
};

public record AccountDetailsResponse(
    Ulid Id,
    Ulid UserId,
    decimal Balance,
    List<AccountTransactionResponse> Transactions)
{
    public DateTimeOffset CreatedAt => Id.Time;
};

public record AccountTransactionResponse(
    Ulid Id,
    decimal Amount,
    AccountTransactionType Type,
    Ulid? AccountFromId,
    Ulid? AccountToId)
{
    public DateTimeOffset CreatedAt => Id.Time;
};