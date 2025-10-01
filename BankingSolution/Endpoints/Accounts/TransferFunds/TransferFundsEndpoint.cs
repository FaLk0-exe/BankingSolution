using BankingSolution.Application.Models.User;
using BankingSolution.Application.Services;
using BankingSolution.Helpers;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BankingSolution.Endpoints.Accounts.TransferFunds;

internal sealed class TransferFundsEndpoint:Endpoint<TransferFundsRequest, Results<Ok<AccountTransactionResponse>, ProblemHttpResult>>
{
    private readonly AccountService _accountService;
    
    public TransferFundsEndpoint(AccountService accountService)
    {
        _accountService = accountService;    
    }

    public override void Configure()
    {
        Post("accounts/transfer-funds");
        AllowAnonymous();
        Description(x => x
            .WithDescription("Transfer funds from one account to another")
            .WithTags("Accounts"));
    }

    public override async Task<Results<Ok<AccountTransactionResponse>, ProblemHttpResult>> ExecuteAsync(TransferFundsRequest req, CancellationToken ct)
    {
        var transferFundsRequest =
            await _accountService.TransferFundsAsync(req.AccountFromId, req.AccountToId, req.Amount, ct);
        
        ProblemHttpResult problemHttpResult = null;

        transferFundsRequest.Switch(
            _ => { },
            error =>
            {
                problemHttpResult = ApplicationLayerErrorTypedResults.Problem(error);
            });

        if (problemHttpResult is not null)
            return problemHttpResult;

        return TypedResults.Ok(transferFundsRequest.AsT0);
    }
}