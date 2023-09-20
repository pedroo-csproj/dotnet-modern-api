using DotNETModernAPI.Application.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace DotNETModernAPI.Infrastructure.CrossCutting.Dependencies;

internal static class AutoMapperConfiguration
{
    public static void AddAutoMapper(this IServiceCollection services) =>
        services.AddAutoMapper(typeof(UserMappers));
}
