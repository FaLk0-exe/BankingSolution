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

    public async Task<List<AccountResponse>> GetAccountsAsync(AccountQueryParams queryParams, CancellationToken ct)
    {
        var query = _dbContext.Accounts.AsNoTracking();

        if (queryParams.AccountIds is not null && queryParams.AccountIds.Any())
            query = query.Where(x => queryParams.AccountIds.Contains(x.Id));

        if (queryParams.UserIds is not null && queryParams.UserIds.Any())
            query = query.Where(x => queryParams.UserIds.Contains(x.Id));

        return await query
            .Skip((int)(queryParams.Page * queryParams.PageSize))
            .Take((int)queryParams.PageSize)
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
                    .OrderByDescending(s=>s.Id)
                    .ToList()
            ))
            .FirstOrDefaultAsync(ct);

        if (account is null)
            return CommonErrors.NotFound(id, typeof(Account));

        return account;
    }

    public async Task<OneOf<AccountDetailsResponse, ApplicationLayerError>> CreateAccountAsync(Ulid userId, decimal initialAmount,
        CancellationToken ct)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, ct);

        if (user is null)
            return CommonErrors.NotFound(userId, typeof(User));

        var accountCreationResult = Account.Create(user, initialAmount);

        if (accountCreationResult.IsT1)
            return CommonErrors.AccountOperationError(accountCreationResult.AsT1);

        var account = accountCreationResult.AsT0;
        
        _dbContext.Accounts.Add(account);
        await _dbContext.SaveChangesAsync(ct);

        return new AccountDetailsResponse(
            account.Id,
            account.User.Id,
            account.Balance,
            account.AccountTransactions.Select(at => new AccountTransactionResponse(
                    at.Id,
                    at.Amount,
                    at.Type,
                    at is ReplenishAccountTransaction
                        ? ((ReplenishAccountTransaction)at).AccountFrom is not null?
                            ((ReplenishAccountTransaction)at).AccountFrom.Id:
                            null
                        : null,
                    at is WithdrawAccountTransaction
                        ? ((WithdrawAccountTransaction)at).AccountTo is not null?
                            ((WithdrawAccountTransaction)at).AccountTo.Id:
                            null
                        : null))
                .ToList());
        }

    public async Task<OneOf<AccountTransactionResponse, ApplicationLayerError>> ReplenishAccountAsync(Ulid accountId,
        decimal amount, CancellationToken ct)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId, ct);

        if (account is null)
            return CommonErrors.NotFound(accountId, typeof(Account));

        var replenishResult = account.Replenish(amount);

        if (replenishResult.IsT1)
            return CommonErrors.AccountOperationError(replenishResult.AsT1);

        await _dbContext.SaveChangesAsync(ct);
        
        var transaction = replenishResult.AsT0.ReplenishAccountTransaction;


        return new AccountTransactionResponse(
            transaction.Id,
            transaction.Amount,
            transaction.Type,
            null,
            null
        );
    }

    public async Task<OneOf<AccountTransactionResponse, ApplicationLayerError>> WithdrawAccountAsync(Ulid accountId,
        decimal amount, CancellationToken ct)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId, ct);

        if (account is null)
            return CommonErrors.NotFound(accountId, typeof(Account));

        var withdrawResult = account.Withdraw(amount);

        if (withdrawResult.IsT1)
            return CommonErrors.AccountOperationError(withdrawResult.AsT1);

        await _dbContext.SaveChangesAsync(ct);

        var transaction = withdrawResult.AsT0.WithdrawAccountTransaction;
        
        return new AccountTransactionResponse(
            transaction.Id,
            transaction.Amount,
            transaction.Type,
            null,
            null
        );
    }

    public async Task<OneOf<AccountTransactionResponse, ApplicationLayerError>> TransferFundsAsync(Ulid accountFromId,
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

        var transaction = transferFundsResult.AsT0.WithdrawAccountTransaction;

        return new AccountTransactionResponse(
            transaction.Id,
            transaction.Amount,
            transaction.Type,
            null,
            transaction.AccountTo!.Id
        );
    }
}