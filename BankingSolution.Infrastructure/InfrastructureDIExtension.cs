using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSolution.Infrastructure;

public static class InfrastructureDIExtension
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
    {
        return services.AddDbContext<BankingDbContext>(opts =>
        {
            opts.UseInMemoryDatabase("BankingSolutionDB");
        });
    }
}