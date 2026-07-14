using GymAPI.Domain.Entities;
using GymAPI.Domain.Interfaces;
using GymAPI.Infrastructure.Persistence.Context;
using GymAPI.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymAPI.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GymDbContext _context;

    public UserRepository(GymDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim());
        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Users.FindAsync(id);
        return entity is null ? null : MapToDomain(entity);
    }

    public async Task AddAsync(User user)
    {
        var entity = MapToEntity(user);
        await _context.Users.AddAsync(entity);
    }

    private static User MapToDomain(UserEntity entity)
    {
        return User.Restore(entity.Id, entity.Email, entity.PasswordHash, entity.FirstName, entity.LastName, entity.CreatedAt, entity.LastLoginAt);
    }

    private static UserEntity MapToEntity(User user)
    {
        return new UserEntity
        {
            Id = user.Id,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }
}
