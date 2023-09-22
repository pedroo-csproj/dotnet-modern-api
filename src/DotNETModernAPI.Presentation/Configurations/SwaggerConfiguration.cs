using Microsoft.OpenApi.Models;

namespace DotNETModernAPI.Presentation.Configurations;

internal static class SwaggerConfiguration
{
    public static void AddSwagger(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSwaggerGen(sgo =>
        {
            sgo.SwaggerDoc("v1", BuildOpenApiInfo(configuration));
            sgo.AddSecurityDefinition("Bearer", BuildOpenApiSecurityScheme());
            sgo.AddSecurityRequirement(BuildOpenApiSecurityRequirement());
        });

    public static void UseSwagger(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseSwagger();
        app.UseSwaggerUI(a => a.SwaggerEndpoint("/swagger/v1/swagger.json", $".NET Modern API - {configuration["Environment"]}:{configuration["BuildId"]}"));
    }

    private static OpenApiInfo BuildOpenApiInfo(IConfiguration configuration) =>
        new()
        {
            Title = $".NET Modern API - {configuration["Environment"]}",
            Version = "1.0.0",
            Description = "Template API for modern .NET Core Projects.",
            Contact = new OpenApiContact
            {
                Name = "Pedro Octávio",
                Email = "pedrooctavio.dev@outlook.com",
                Url = new Uri("https://github.com/pedroo-csproj")
            },
            License = new OpenApiLicense()
            {
                Name = "MIT",
                Url = new Uri("https://github.com/pedroo-csproj/dotnet-modern-api/blob/main/LICENSE")
            }
        };

    private static OpenApiSecurityScheme BuildOpenApiSecurityScheme() =>
        new()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Authorization header using the Bearer scheme."
        };

    private static OpenApiSecurityRequirement BuildOpenApiSecurityRequirement() =>
        new()
        {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
        };
}
