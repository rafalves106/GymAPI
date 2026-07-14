using GymAPI.Application.Interfaces;
using GymAPI.Application.Services;
using GymAPI.Domain.Interfaces;
using GymAPI.Infrastructure.Authentication;
using GymAPI.Infrastructure.Persistence.Context;
using GymAPI.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymAPI.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<GymDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITrainingRepository, TrainingRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Authentication
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IExerciseUseCases, ExerciseService>();
        services.AddScoped<IAuthUseCases, AuthService>();
        services.AddScoped<ITrainingUseCases, TrainingService>();

        return services;
    }
}
