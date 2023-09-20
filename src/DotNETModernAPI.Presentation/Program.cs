using DotNETModernAPI.Infrastructure.CrossCutting;
using DotNETModernAPI.Presentation.Configurations;
using DotNETModernAPI.Presentation.Policies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddAuthorization(ao => ao.AddUsersOptions());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger(builder.Configuration);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
