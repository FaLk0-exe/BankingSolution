using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using BankingSolution.Application.Models.User;
using BankingSolution.Domain.Enums;
using Cysharp.Serialization.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace BankingSolution.Tests.Integration;

public class AccountTests : ApiTestBase
{
    private JsonSerializerOptions _jsonSerializerOptions;
    public AccountTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        _jsonSerializerOptions.Converters.Add(new UlidJsonConverter());
    }
    
    [Fact]
    public async Task CreateAccount_ShouldReturn201()
    {
        var userResp = await Client.PostAsJsonAsync("/api/users", new { firstName = "Alice", lastName = "Smith" });
        var user = await userResp.Content.ReadFromJsonAsync<UserResponse>(_jsonSerializerOptions);

        var request = new { userId = user!.Id, initialBalance = 1000m };

        var response = await Client.PostAsJsonAsync("/api/accounts", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var account = await response.Content.ReadFromJsonAsync<AccountDetailsResponse>(_jsonSerializerOptions);
        account.Should().NotBeNull();
        account!.Balance.Should().Be(1000m);
    }

    [Fact]
    public async Task GetAccounts_ShouldReturnList()
    {
        var response = await Client.GetAsync("/api/accounts?pageSize=10&page=1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var accounts = await response.Content.ReadFromJsonAsync<List<AccountResponse>>(_jsonSerializerOptions);
        accounts.Should().NotBeNull();
    }

    [Fact]
    public async Task Replenish_ShouldIncreaseBalance()
    {
        var userResp = await Client.PostAsJsonAsync("/api/users", new { firstName = "Bob", lastName = "Test" });
        var user = await userResp.Content.ReadFromJsonAsync<UserResponse>(_jsonSerializerOptions);

        var accResp = await Client.PostAsJsonAsync("/api/accounts", new { userId = user!.Id, initialBalance = 100m });
        var account = await accResp.Content.ReadFromJsonAsync<AccountDetailsResponse>(_jsonSerializerOptions);

        var response = await Client.PostAsJsonAsync($"/api/accounts/{account!.Id}/replenish", new { amount = 50m });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tx = await response.Content.ReadFromJsonAsync<AccountTransactionResponse>(_jsonSerializerOptions);
        tx!.Amount.Should().Be(50m);
        tx.Type.Should().Be(AccountTransactionType.Replenish);
    }

    [Fact]
    public async Task Withdraw_ShouldDecreaseBalance()
    {
        var userResp = await Client.PostAsJsonAsync("/api/users", new { firstName = "Tom", lastName = "Lee" });
        var user = await userResp.Content.ReadFromJsonAsync<UserResponse>(_jsonSerializerOptions);

        var accResp = await Client.PostAsJsonAsync("/api/accounts", new { userId = user!.Id, initialBalance = 200m });
        var account = await accResp.Content.ReadFromJsonAsync<AccountDetailsResponse>(_jsonSerializerOptions);

        var response = await Client.PostAsJsonAsync($"/api/accounts/{account!.Id}/withdraw", new { amount = 50m });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tx = await response.Content.ReadFromJsonAsync<AccountTransactionResponse>(_jsonSerializerOptions);
        tx!.Amount.Should().Be(-50m);
        tx.Type.Should().Be(AccountTransactionType.Withdraw);
    }

    [Fact]
    public async Task TransferFunds_ShouldWork()
    {
        var user1 = await (await Client.PostAsJsonAsync("/api/users", new { firstName = "U1", lastName = "A" }))
            .Content.ReadFromJsonAsync<UserResponse>(_jsonSerializerOptions);
        var user2 = await (await Client.PostAsJsonAsync("/api/users", new { firstName = "U2", lastName = "B" }))
            .Content.ReadFromJsonAsync<UserResponse>(_jsonSerializerOptions);

        var acc1 = await (await Client.PostAsJsonAsync("/api/accounts", new { userId = user1!.Id, initialBalance = 300m }))
            .Content.ReadFromJsonAsync<AccountDetailsResponse>(_jsonSerializerOptions);
        var acc2 = await (await Client.PostAsJsonAsync("/api/accounts", new { userId = user2!.Id, initialBalance = 100m }))
            .Content.ReadFromJsonAsync<AccountDetailsResponse>(_jsonSerializerOptions);

        var response = await Client.PostAsJsonAsync("/api/accounts/transfer-funds", new
        {
            accountFromId = acc1!.Id,
            accountToId = acc2!.Id,
            amount = 50m
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tx = await response.Content.ReadFromJsonAsync<AccountTransactionResponse>(_jsonSerializerOptions);
        tx.Should().NotBeNull();
        tx!.Amount.Should().Be(-50m);
    }

    [Fact]
    public async Task GetAccountDetails_ShouldReturn200()
    {
        var userResp = await Client.PostAsJsonAsync("/api/users", new { firstName = "Dan", lastName = "Brown" });
        var user = await userResp.Content.ReadFromJsonAsync<UserResponse>(_jsonSerializerOptions);

        var accResp = await Client.PostAsJsonAsync("/api/accounts", new { userId = user!.Id, initialBalance = 500m });
        var account = await accResp.Content.ReadFromJsonAsync<AccountDetailsResponse>(_jsonSerializerOptions);

        var response = await Client.GetAsync($"/api/accounts/{account!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var accDetails = await response.Content.ReadFromJsonAsync<AccountDetailsResponse>(_jsonSerializerOptions);
        accDetails.Should().NotBeNull();
        accDetails!.Balance.Should().Be(500m);
    }

    [Fact]
    public async Task GetAccount_WithInvalidId_ShouldReturn404()
    {
        var response = await Client.GetAsync($"/api/accounts/{Ulid.NewUlid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Replenish_WithInvalidId_ShouldReturn404()
    {
        var response = await Client.PostAsJsonAsync($"/api/accounts/{Ulid.NewUlid()}/replenish", new { amount = 100m });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Withdraw_WithInvalidId_ShouldReturn404()
    {
        var response = await Client.PostAsJsonAsync($"/api/accounts/{Ulid.NewUlid()}/withdraw", new { amount = 50m });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TransferFunds_WithInvalidAccount_ShouldReturn404()
    {
        var req = new
        {
            accountFromId = Ulid.NewUlid().ToString(),
            accountToId = Ulid.NewUlid().ToString(),
            amount = 10m
        };

        var response = await Client.PostAsJsonAsync("/api/accounts/transfer-funds", req);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Withdraw_MoreThanBalance_ShouldReturn409()
    {
        var userResp = await Client.PostAsJsonAsync("/api/users", new { firstName = "Jack", lastName = "Test" });
        var user = await userResp.Content.ReadFromJsonAsync<UserResponse>(_jsonSerializerOptions);
        
        var accResp = await Client.PostAsJsonAsync("/api/accounts", new { userId = user!.Id, initialBalance = 100m });
        var account = await accResp.Content.ReadFromJsonAsync<AccountDetailsResponse>(_jsonSerializerOptions);
        
        var response = await Client.PostAsJsonAsync($"/api/accounts/{account!.Id}/withdraw", new { amount = 200m });
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task TransferFunds_WithoutEnoughBalance_ShouldReturn409()
    {
        var u1 = await (await Client.PostAsJsonAsync("/api/users", new { firstName = "T1", lastName = "A" }))
            .Content.ReadFromJsonAsync<UserResponse>(_jsonSerializerOptions);
        var u2 = await (await Client.PostAsJsonAsync("/api/users", new { firstName = "T2", lastName = "B" }))
            .Content.ReadFromJsonAsync<UserResponse>(_jsonSerializerOptions);
        
        var acc1 = await (await Client.PostAsJsonAsync("/api/accounts", new { userId = u1!.Id, initialBalance = 50m }))
            .Content.ReadFromJsonAsync<AccountDetailsResponse>(_jsonSerializerOptions);
        var acc2 = await (await Client.PostAsJsonAsync("/api/accounts", new { userId = u2!.Id, initialBalance = 100m }))
            .Content.ReadFromJsonAsync<AccountDetailsResponse>(_jsonSerializerOptions);
        
        var req = new
        {
            accountFromId = acc1!.Id,
            accountToId = acc2!.Id,
            amount = 200m
        };

        var response = await Client.PostAsJsonAsync("/api/accounts/transfer-funds", req);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}