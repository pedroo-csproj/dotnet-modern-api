using DotNETModernAPI.Infrastructure.CrossCutting.Dependencies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNETModernAPI.Infrastructure.CrossCutting;

public static class Bootsrapper
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataContext(configuration);
        services.AddDomainServices();
        services.AddEmail(configuration);
        services.AddFluentValidation();
        services.AddIdentity();
        services.AddMediator();
        services.AddProviders();
    }
}
