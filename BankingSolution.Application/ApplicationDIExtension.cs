using BankingSolution.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSolution.Application;

public static class ApplicationDIExtension
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        return services
            .AddScoped<AccountService>()
            .AddScoped<UserService>();
    }
}