using DotNETModernAPI.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNETModernAPI.Infrastructure.CrossCutting.Dependencies;

internal static class EmailConfiguration
{
    public static void AddEmail(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<EmailSettingsModel>(es => configuration.GetSection("Email").Bind(es));
}
