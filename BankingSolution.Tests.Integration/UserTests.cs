using System.Net;
using System.Net.Http.Json;
using BankingSolution.Application.Models.User;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace BankingSolution.Tests.Integration;

public class UsersTests : ApiTestBase
{
    public UsersTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task CreateUser_ShouldReturnUser()
    {
        var request = new { firstName = "John", lastName = "Doe" };

        var response = await Client.PostAsJsonAsync("/api/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().NotBeNull();
        user!.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetUsers_ShouldReturnList()
    {
        var response = await Client.GetAsync("/api/users?pageSize=10&pageNumber=0");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        users.Should().NotBeNull();
    }
}
