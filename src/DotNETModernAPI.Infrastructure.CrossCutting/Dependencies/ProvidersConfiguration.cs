using DotNETModernAPI.Domain.Providers;
using DotNETModernAPI.Infrastructure.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace DotNETModernAPI.Infrastructure.CrossCutting.Dependencies;

internal static class ProvidersConfiguration
{
    public static void AddProviders(this IServiceCollection services) =>
        services.AddTransient<IEmailProvider, EmailProvider>();
}
