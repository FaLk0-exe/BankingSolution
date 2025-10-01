namespace BankingSolution.Endpoints.Accounts.CreateAccount;

internal sealed record CreateAccountRequest(Ulid UserId, decimal InitialBalance);