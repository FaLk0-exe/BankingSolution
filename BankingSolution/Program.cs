using BankingSolution;
using BankingSolution.Application;
using BankingSolution.Infrastructure;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructureDependencies()
    .AddApplicationDependencies()
    .AddApiDependencies();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
});
app.UseSwaggerGen();

await app.RunAsync();
public partial class Program { }
