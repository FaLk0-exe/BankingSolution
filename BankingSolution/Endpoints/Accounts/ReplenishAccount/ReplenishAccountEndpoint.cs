using BankingSolution.Application.Models.User;
using BankingSolution.Application.Services;
using BankingSolution.Endpoints.Accounts.CreateAccount;
using BankingSolution.Helpers;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BankingSolution.Endpoints.Accounts.ReplenishAccount;

internal sealed class ReplenishAccountEndpoint:Endpoint<ReplenishAccountRequest, Results<Ok<AccountTransactionResponse>, ProblemHttpResult>>
{
    private readonly AccountService _accountService;
    
    public ReplenishAccountEndpoint(AccountService accountService)
    {
        _accountService = accountService;    
    }

    public override void Configure()
    {
        Post("accounts/{id}/replenish");
        AllowAnonymous();
        Description(x => x
            .WithDescription("Repleishes an account")
            .WithTags("Accounts"));
    }

    public override async Task<Results<Ok<AccountTransactionResponse>, ProblemHttpResult>> ExecuteAsync(ReplenishAccountRequest req, CancellationToken ct)
    {
        var accountId = Route<Ulid>("id");

        var replenishAccountResult = await _accountService.ReplenishAccountAsync(accountId, req.Amount, ct);
        
        ProblemHttpResult problemHttpResult = null;

        replenishAccountResult.Switch(
            _ => { },
            error =>
            {
                problemHttpResult = ApplicationLayerErrorTypedResults.Problem(error);
            });

        if (problemHttpResult is not null)
            return problemHttpResult;

        return TypedResults.Ok(replenishAccountResult.AsT0);
    }
}