namespace BankingSolution.Utilities.DomainError;

public enum DomainErrorKind { NotFound, BusinessRule, Conflict, Unexpected }

public record struct DomainError(
    string Code,
    DomainErrorKind Kind,
    string? Message = null,
    object? Meta = null
);