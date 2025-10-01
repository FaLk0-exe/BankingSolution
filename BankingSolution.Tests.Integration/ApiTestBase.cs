using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace BankingSolution.Tests.Integration;

using System.Net.Http;
using Xunit;

public class ApiTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;

    protected ApiTestBase(WebApplicationFactory<Program> factory)
    {
        Client = factory.CreateClient();
    }
}