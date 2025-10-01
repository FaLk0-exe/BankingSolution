using BankingSolution.Application.Filters;
using BankingSolution.Application.Models.User;
using BankingSolution.Application.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BankingSolution.Endpoints.Accounts.GetAccounts;

internal sealed class GetAccountsEndpoint:Endpoint<AccountQueryParams, Results<Ok<List<AccountResponse>>, ProblemHttpResult>>
{
    private readonly AccountService _accountService;
    
    public GetAccountsEndpoint(AccountService accountService)
    {
        _accountService = accountService;
    }

    public override void Configure()
    {
        Get("accounts");
        AllowAnonymous();
        Description(x => x
            .WithDescription("Returns all accounts")
            .WithTags("Accounts"));
    }

    public override async Task<Results<Ok<List<AccountResponse>>, ProblemHttpResult>> ExecuteAsync(
        AccountQueryParams queryParams, CancellationToken ct)
    {
        var response = await _accountService.GetAccountsAsync(queryParams, ct);

        return TypedResults.Ok(response);
    }
}