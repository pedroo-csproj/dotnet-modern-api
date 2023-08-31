using Microsoft.Extensions.DependencyInjection;

namespace DotNETModernAPI.Infrastructure.CrossCutting.Dependencies;

internal static class MediatorConfiguration
{
    public static void AddMediator(this IServiceCollection services) =>
        services.AddMediatR(msc =>
            msc.RegisterServicesFromAssembly(AppDomain.CurrentDomain.Load("DotNETModernAPI.Application")));
}
