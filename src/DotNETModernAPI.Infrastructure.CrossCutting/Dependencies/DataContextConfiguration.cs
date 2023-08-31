using DotNETModernAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNETModernAPI.Infrastructure.CrossCutting.Dependencies;

internal static class DataContextConfiguration
{
    public static void AddDataContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<ApplicationDataContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"),
                actions => actions.MigrationsAssembly("DotNETModernAPI.Infrastructure.Data.Migrations")));
}
