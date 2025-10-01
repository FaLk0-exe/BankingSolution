using BankingSolution.Application.Models.User;
using BankingSolution.Application.Services;
using BankingSolution.Helpers;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BankingSolution.Endpoints.Accounts.GetAccountDetails;

internal sealed class
    GetAccountDetailsEndpoint : EndpointWithoutRequest<Results<Ok<AccountDetailsResponse>, ProblemHttpResult>>
{
    private readonly AccountService _accountService;

    public GetAccountDetailsEndpoint(AccountService accountService)
    {
        _accountService = accountService;
    }

    public override void Configure()
    {
        Get("accounts/{id}");
        AllowAnonymous();
        Description(x => x
            .WithDescription("Returns account details")
            .WithTags("Accounts")
            .WithName("GetAccountDetails"));
    }

    public override async Task<Results<Ok<AccountDetailsResponse>, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Ulid>("id");

        var response = await _accountService.GetAccountByIdAsync(id, ct);

        ProblemHttpResult problemHttpResult = null;

        response.Switch(
            _ => { },
            error =>
            {
                problemHttpResult = ApplicationLayerErrorTypedResults.Problem(error);
            });

        if (problemHttpResult is not null)
            return problemHttpResult;

        return TypedResults.Ok(response.AsT0);
    }
}