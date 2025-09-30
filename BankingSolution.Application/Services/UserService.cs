using BankingSolution.Application.Models.User;
using BankingSolution.Domain.Entities;
using BankingSolution.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BankingSolution.Application.Services;

public class UserService
{
    private readonly BankingDbContext  _dbContext;

    public UserService(BankingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Ulid> CreateUserAsync(string firstName, string lastName, CancellationToken ct)
    {
        var user = new User(firstName, lastName);

        await _dbContext.SaveChangesAsync(ct);

        return user.Id;
    }

    public async Task<List<UserResponse>> GetUsersAsync(uint pageNumber, uint pageSize, CancellationToken ct)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Skip((int)(pageNumber * pageSize))
            .Take((int)pageNumber)
            .Select(s => new UserResponse(s.Id, s.FirstName, s.LastName))
            .ToListAsync(ct);
    }
}