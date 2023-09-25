using DotNETModernAPI.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DotNETModernAPI.Infrastructure.CrossCutting.Dependencies;

internal static class DomainServicesConfiguration
{
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddTransient<RoleServices>();
        services.AddTransient<UserServices>();
    }
}
