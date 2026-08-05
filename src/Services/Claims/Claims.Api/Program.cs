using Claims.Api.Middleware;
using Claims.Application;
using Claims.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(
            "AngularDevelopment",
            policy =>
            {
                policy
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddClaimsApplication();

builder.Services.AddClaimsInfrastructure(
    builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AngularDevelopment");

app.MapControllers();

app.MapGet(
    "/health",
    () => Results.Ok(
        new
        {
            service = "claims-api",
            status = "healthy",
            timestamp = DateTimeOffset.UtcNow
        }));

app.Run();

public partial class Program;