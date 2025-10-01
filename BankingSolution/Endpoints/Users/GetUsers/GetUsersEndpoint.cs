using BankingSolution.Application.Filters;
using BankingSolution.Application.Models.User;
using BankingSolution.Application.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BankingSolution.Endpoints.Users.GetUsers;

internal sealed class GetUsersEndpoint:Endpoint<UserQueryParams, Results<Ok<List<UserResponse>>, ProblemHttpResult>>
{
    private readonly UserService _userService;
    
    public GetUsersEndpoint(UserService userService)
    {
        _userService = userService;
    }

    public override void Configure()
    {
        Get("users");
        AllowAnonymous();
        Description(x => x
            .WithDescription("Returns all users")
            .WithTags("Users"));
    }

    public override async Task<Results<Ok<List<UserResponse>>, ProblemHttpResult>> ExecuteAsync(
        UserQueryParams queryParams, CancellationToken ct)
    {
        var response = await _userService.GetUsersAsync(queryParams, ct);

        return TypedResults.Ok(response);
    }
}