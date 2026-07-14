using GymAPI.Infrastructure.Persistence.Context;
using GymAPI.Infrastructure.Persistence.Entities;
using GymAPI.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace GymAPI.Infrastructure.Persistence;

public class DbSeeder
{
    private readonly AppDbContext _context;

    public DbSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        var masterUsername = Environment.GetEnvironmentVariable("MASTER_USERNAME");
        var masterPassword = Environment.GetEnvironmentVariable("MASTER_PASSWORD");
        var masterEmail = Environment.GetEnvironmentVariable("MASTER_EMAIL");

        if (string.IsNullOrWhiteSpace(masterUsername) || string.IsNullOrWhiteSpace(masterPassword))
            return;

        var existing = await _context.Users.FirstOrDefaultAsync(u => u.Username == masterUsername);
        if (existing is not null)
            return;

        var hasher = new BcryptPasswordHasher();
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Username = masterUsername,
            Email = masterEmail ?? $"{masterUsername}@master.local",
            PasswordHash = hasher.Hash(masterPassword),
            IsMaster = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
