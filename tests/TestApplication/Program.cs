using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().AddOpenApi(
    options =>
    {
        options.CreateHealthCheckOpenApiDocument = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("/healthz")
    .WithApiDescription<HealthReport>(
        metadata =>
        {
            metadata.Summary = "Returns information about the health of the system";
        });
app.MapControllers();

app.Run();
