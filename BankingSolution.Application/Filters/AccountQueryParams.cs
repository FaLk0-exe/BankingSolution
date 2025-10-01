namespace BankingSolution.Application.Filters;

public class AccountQueryParams
{
    public List<Ulid>? UserIds { get; init; }
    public List<Ulid>? AccountIds { get; init; }
    public uint PageSize { get; init; }
    public uint Page { get; init; }
}