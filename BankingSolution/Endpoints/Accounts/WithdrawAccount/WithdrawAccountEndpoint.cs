using BankingSolution.Application.Models.User;
using BankingSolution.Application.Services;
using BankingSolution.Helpers;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BankingSolution.Endpoints.Accounts.WithdrawAccount;

internal sealed class WithdrawAccountEndpoint:Endpoint<WithdrawAccountRequest, Results<Ok<AccountTransactionResponse>, ProblemHttpResult>>
{
    private readonly AccountService _accountService;
    
    public WithdrawAccountEndpoint(AccountService accountService)
    {
        _accountService = accountService;    
    }

    public override void Configure()
    {
        Post("accounts/{id}/withdraw");
        AllowAnonymous();
        Description(x => x
            .WithDescription("Withdraw funds from an account")
            .WithTags("Accounts"));
    }

    public override async Task<Results<Ok<AccountTransactionResponse>, ProblemHttpResult>> ExecuteAsync(WithdrawAccountRequest req, CancellationToken ct)
    {
        var accountId = Route<Ulid>("id");

        var withdrawAccountResult = await _accountService.WithdrawAccountAsync(accountId, req.Amount, ct);
        
        ProblemHttpResult problemHttpResult = null;

        withdrawAccountResult.Switch(
            _ => { },
            error =>
            {
                problemHttpResult = ApplicationLayerErrorTypedResults.Problem(error);
            });

        if (problemHttpResult is not null)
            return problemHttpResult;

        return TypedResults.Ok(withdrawAccountResult.AsT0);
    }
}