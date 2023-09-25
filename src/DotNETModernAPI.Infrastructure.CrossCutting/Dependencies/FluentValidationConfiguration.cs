using DotNETModernAPI.Domain.Entities;
using DotNETModernAPI.Domain.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DotNETModernAPI.Infrastructure.CrossCutting.Dependencies;

internal static class FluentValidationConfiguration
{
    public static void AddFluentValidation(this IServiceCollection services)
    {
        services.AddTransient<IValidator<Role>, RoleValidator>();
        services.AddTransient<IValidator<User>, UserValidator>();
    }
}
