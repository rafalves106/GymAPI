using GymAPI.Domain.Entities;
using GymAPI.Domain.Interfaces;
using GymAPI.Infrastructure.Persistence.Context;
using GymAPI.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymAPI.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim());
        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Username == username.Trim());
        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
    {
        var input = usernameOrEmail.Trim();
        var isEmail = input.Contains('@');

        var entity = isEmail
            ? await _context.Users.FirstOrDefaultAsync(u => u.Email == input.ToLower())
            : await _context.Users.FirstOrDefaultAsync(u => u.Username == input);

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
        return User.Restore(
            entity.Id,
            entity.Username,
            entity.Email,
            entity.PasswordHash,
            entity.IsMaster,
            entity.CreatedAt,
            entity.LastLoginAt);
    }

    private static UserEntity MapToEntity(User user)
    {
        return new UserEntity
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            IsMaster = user.IsMaster,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }
}
