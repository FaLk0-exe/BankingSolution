namespace BankingSolution.Domain.Entities;

public class User
{
    public Ulid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    private readonly List<Account> _accounts = new();
    public IReadOnlyList<Account> Accounts => _accounts;

    public User(string firstName, string lastName)
    {
        Id = Ulid.NewUlid();
        FirstName = firstName;
        LastName = lastName;
    }
}