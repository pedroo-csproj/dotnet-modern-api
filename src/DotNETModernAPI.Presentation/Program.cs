using DotNETModernAPI.Infrastructure.CrossCutting;
using DotNETModernAPI.Presentation.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddDependencies(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger(builder.Configuration);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
