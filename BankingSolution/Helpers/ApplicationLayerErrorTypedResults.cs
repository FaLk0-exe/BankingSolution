using System.Net;
using System.Reflection;
using BankingSolution.Application.ApplicationError;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BankingSolution.Helpers;

public static class ApplicationLayerErrorTypedResults
{
    public static ProblemHttpResult Problem(ApplicationLayerError applicationLayerError)
    {
        return TypedResults.Problem(
            title: applicationLayerError.Code,
            statusCode: ConvertDomainErrorKindToHttpCode(applicationLayerError.Kind),
            extensions:ConvertErrorMetadataToDictionary(applicationLayerError.Meta));
    }

    private static int ConvertDomainErrorKindToHttpCode(ApplicationErrorKind applicationErrorKind)
    {
        return (int)(applicationErrorKind switch
        {
            ApplicationErrorKind.BusinessRule => HttpStatusCode.Conflict,
            ApplicationErrorKind.NotFound => HttpStatusCode.NotFound,
            _ => throw new ArgumentOutOfRangeException(nameof(applicationErrorKind), applicationErrorKind, null)
        });
    }

    private static Dictionary<string, object?> ConvertErrorMetadataToDictionary(object? metadata)
    {
        var dict = new Dictionary<string, object?>();

        if (metadata == null)
            return dict;

        var type = metadata.GetType();
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.CanRead)
            {
                var value = prop.GetValue(metadata, null);
                dict[prop.Name] = value;
            }
        }

        return dict;
    }
}