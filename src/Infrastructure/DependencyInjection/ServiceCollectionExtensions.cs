using GymAPI.Application.Interfaces;
using GymAPI.Application.Services;
using GymAPI.Domain.Interfaces;
using GymAPI.Infrastructure.Persistence;
using GymAPI.Infrastructure.Persistence.Context;
using GymAPI.Infrastructure.Persistence.Repositories;
using GymAPI.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymAPI.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWorkoutRepository, WorkoutRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IJwtTokenService, JwtProvider>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthUseCases, AuthService>();
        services.AddScoped<IWorkoutUseCases, WorkoutService>();
        services.AddScoped<ISessionUseCases, SessionService>();

        return services;
    }
}
