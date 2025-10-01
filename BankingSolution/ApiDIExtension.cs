using System.Text.Json;
using System.Text.Json.Serialization;
using BankingSolution.Mappers;
using Cysharp.Serialization.Json;
using FastEndpoints;
using FastEndpoints.Swagger;
using NJsonSchema.Generation.TypeMappers;
using NSwag;

namespace BankingSolution;

public static class ApiDIExtension
{
    public static IServiceCollection AddApiDependencies(this IServiceCollection services)
    {
        return services
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.SerializerOptions.Converters.Add(new UlidJsonConverter());
            })
            .AddFastEndpoints().SwaggerDocument(configuration =>
            {
                configuration.EnableJWTBearerAuth = false;
                configuration.DocumentSettings = settings =>
                {
                    settings.Title = "Banking Solution API";
                    settings.SchemaSettings.TypeMappers.Add(new UlidTypeMapper());
                };
            });
    }
}