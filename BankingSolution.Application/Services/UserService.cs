using BankingSolution.Application.Filters;
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

    public async Task<User> CreateUserAsync(string firstName, string lastName, CancellationToken ct)
    {
        var user = new User(firstName, lastName);
        
        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync(ct);

        return user;
    }

    public async Task<List<UserResponse>> GetUsersAsync(UserQueryParams queryParams, CancellationToken ct)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Skip((int)(queryParams.PageNumber * queryParams.PageSize))
            .Take((int)queryParams.PageNumber)
            .Select(s => new UserResponse(s.Id, s.FirstName, s.LastName))
            .ToListAsync(ct);
    }
}