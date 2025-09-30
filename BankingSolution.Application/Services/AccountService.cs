using BankingSolution.Application.ApplicationError;
using BankingSolution.Application.Filters;
using BankingSolution.Application.Models.User;
using BankingSolution.Domain.Entities;
using BankingSolution.Domain.Indicators;
using BankingSolution.Domain.Services;
using BankingSolution.Infrastructure;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace BankingSolution.Application.Services;

public class AccountService
{
    private readonly BankingDbContext _dbContext;
    private readonly TransferService _transferService;


    public AccountService(BankingDbContext dbContext, TransferService transferService)
    {
        _dbContext = dbContext;
        _transferService = transferService;
    }

    public async Task<List<AccountResponse>> GetAccountsAsync(AccountQueryFilter queryFilter, CancellationToken ct)
    {
        var query = _dbContext.Accounts.AsNoTracking();

        if (queryFilter.AccountIds.Any())
            query = query.Where(x => queryFilter.AccountIds.Contains(x.Id));

        if (queryFilter.UserIds.Any())
            query = query.Where(x => queryFilter.UserIds.Contains(x.Id));

        return await query
            .Skip((int)(queryFilter.Page * queryFilter.PageSize))
            .Take((int)queryFilter.PageSize)
            .Select(x => new AccountResponse(x.Id, x.User.Id, x.AccountTransactions.Sum(a => a.Amount)))
            .ToListAsync(ct);
    }

    public async Task<OneOf<AccountDetailsResponse, ApplicationLayerError>> GetAccountByIdAsync(Ulid id,
        CancellationToken ct)
    {
        var account = await _dbContext.Accounts
            .Where(x => x.Id == id)
            .Select(x => new AccountDetailsResponse(
                x.Id,
                x.User.Id,
                x.AccountTransactions.Sum(a => a.Amount),
                x.AccountTransactions
                    .Select(at =>
                        new AccountTransactionResponse(
                            at.Id,
                            at.Amount,
                            at.Type,
                            at is ReplenishAccountTransaction
                                ? ((ReplenishAccountTransaction)at).AccountFrom!.Id
                                : null,
                            at is WithdrawAccountTransaction
                                ? ((WithdrawAccountTransaction)at).AccountTo!.Id
                                : null)
                    )
                    .ToList()
            ))
            .FirstOrDefaultAsync(ct);

        if (account is null)
            return CommonErrors.NotFound(id, typeof(Account));

        return account;
    }

    public async Task<OneOf<Account, ApplicationLayerError>> CreateAccountAsync(Ulid userId, decimal initialAmount,
        CancellationToken ct)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, ct);

        if (user is null)
            return CommonErrors.NotFound(userId, typeof(User));

        var accountCreationResult = Account.Create(user, initialAmount);

        if (accountCreationResult.IsT1)
            return CommonErrors.AccountOperationError(accountCreationResult.AsT1);

        await _dbContext.SaveChangesAsync(ct);

        return accountCreationResult.AsT0;
    }

    public async Task<OneOf<AccountReplenishSucceeded, ApplicationLayerError>> ReplenishAccountAsync(Ulid accountId,
        decimal amount, CancellationToken ct)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId, ct);

        if (account is null)
            return CommonErrors.NotFound(accountId, typeof(Account));

        var replenishResult = account.Replenish(amount);

        if (replenishResult.IsT1)
            return CommonErrors.AccountOperationError(replenishResult.AsT1);

        await _dbContext.SaveChangesAsync(ct);

        return replenishResult.AsT0;
    }

    public async Task<OneOf<AccountWithdrawSucceeded, ApplicationLayerError>> WithdrawAccountAsync(Ulid accountId,
        decimal amount, CancellationToken ct)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId, ct);

        if (account is null)
            return CommonErrors.NotFound(accountId, typeof(Account));

        var withdrawResult = account.Withdraw(amount);

        if (withdrawResult.IsT1)
            return CommonErrors.AccountOperationError(withdrawResult.AsT1);

        await _dbContext.SaveChangesAsync(ct);

        return withdrawResult.AsT0;
    }

    public async Task<OneOf<TransferSucceeded, ApplicationLayerError>> TransferFundsAsync(Ulid accountFromId,
        Ulid accountToId, decimal amount, CancellationToken ct)
    {
        var accountFrom = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountFromId, ct);

        if (accountFrom is null)
            return CommonErrors.NotFound(accountFromId, typeof(Account));

        var accountTo = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountToId, ct);
        if (accountTo is null)
            return CommonErrors.NotFound(accountToId, typeof(Account));

        var transferFundsResult = _transferService.TransferFunds(accountFrom, accountTo, amount);

        if (transferFundsResult.IsT1)
            return CommonErrors.AccountOperationError(transferFundsResult.AsT1);

        await _dbContext.SaveChangesAsync(ct);

        return transferFundsResult.AsT0;
    }
}