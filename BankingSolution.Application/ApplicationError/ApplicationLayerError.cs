namespace BankingSolution.Application.ApplicationError;

public enum ApplicationErrorKind { NotFound, BusinessRule }

public record struct ApplicationLayerError(
    string Code,
    ApplicationErrorKind Kind,
    object? Meta = null
);