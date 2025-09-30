namespace BankingSolution.Application.Models.User;

public record struct UserResponse(Ulid Id, string FirstName, string LastName);