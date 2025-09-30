using BankingSolution.Domain.Indicators;

namespace BankingSolution.Application.ApplicationError;

public static class CommonErrors
{
    public static ApplicationLayerError NotFound(Ulid id, Type type) =>
        new ApplicationLayerError($"{type.BaseType}NotFound", ApplicationErrorKind.NotFound, new { Id = id });

    public static ApplicationLayerError AccountOperationError(IDomainError domainError) =>
        new ApplicationLayerError(domainError.GetType().BaseType!.ToString(), ApplicationErrorKind.BusinessRule,
            domainError);
}