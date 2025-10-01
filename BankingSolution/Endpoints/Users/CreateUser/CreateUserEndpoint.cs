using BankingSolution.Application.Models.User;
using BankingSolution.Application.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BankingSolution.Endpoints.Users.CreateUser;

internal sealed class CreateUserEndpoint:Endpoint<CreateUserRequest, Results<Ok<UserResponse>, ProblemHttpResult>>
{
    private readonly UserService  _userService;
    
    public CreateUserEndpoint(UserService userService)
    {
        _userService = userService;
    }

    public override void Configure()
    {
        Post("users");
        AllowAnonymous();
        Description(x => x
            .WithDescription("Creates a new user")
            .WithTags("Users"));
    }

    public override async Task<Results<Ok<UserResponse>, ProblemHttpResult>> ExecuteAsync(CreateUserRequest req,
        CancellationToken ct)
    {
        var user = await _userService.CreateUserAsync(req.FirstName, req.LastName, ct);
        
        return TypedResults.Ok(new UserResponse(user.Id, user.FirstName, user.LastName));
    }
}