namespace BankingSolution.Endpoints.Accounts.TransferFunds;

internal sealed record TransferFundsRequest(Ulid AccountFromId, Ulid AccountToId, decimal Amount);