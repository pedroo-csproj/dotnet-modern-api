using DotNETModernAPI.Infrastructure.CrossCutting;
using DotNETModernAPI.Infrastructure.Data;
using DotNETModernAPI.Presentation.Configurations;
using DotNETModernAPI.Presentation.Policies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddAuthorization(ao => ao.AddUsersOptions());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var applicationDataContext = scope.ServiceProvider.GetRequiredService<ApplicationDataContext>();

    await applicationDataContext.Database.MigrateAsync();
}

app.UseSwagger(builder.Configuration);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
