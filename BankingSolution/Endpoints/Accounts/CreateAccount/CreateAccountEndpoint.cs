using BankingSolution.Application.Models.User;
using BankingSolution.Application.Services;
using BankingSolution.Helpers;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BankingSolution.Endpoints.Accounts.CreateAccount;

internal sealed class CreateAccountEndpoint:Endpoint<CreateAccountRequest, Results<CreatedAtRoute<AccountDetailsResponse>, ProblemHttpResult>>
{
    private readonly AccountService _accountService;
    
    public CreateAccountEndpoint(AccountService accountService)
    {
        _accountService = accountService;    
    }
    
    public override void Configure()
    {
        Post("accounts");
        AllowAnonymous();
        Description(x => x
            .WithDescription("Creates a new account")
            .WithTags("Accounts"));
    }

    public override async Task<Results<CreatedAtRoute<AccountDetailsResponse>, ProblemHttpResult>> ExecuteAsync(CreateAccountRequest req, CancellationToken ct)
    {
        var accountCreationResult = await _accountService.CreateAccountAsync(req.UserId, req.InitialBalance, ct);
        
        ProblemHttpResult problemHttpResult = null;

        accountCreationResult.Switch(
            _ => { },
            error =>
            {
                problemHttpResult = ApplicationLayerErrorTypedResults.Problem(error);
            });

        if (problemHttpResult is not null)
            return problemHttpResult;

        var account = accountCreationResult.AsT0;

        return TypedResults.CreatedAtRoute(
            routeName: "GetAccountDetails",
            routeValues: new { Id = account.Id },
            value: account);
    }
}