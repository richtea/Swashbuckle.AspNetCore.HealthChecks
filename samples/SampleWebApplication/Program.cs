using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

/////////////////////////////////////////////////////////////////
// Configure the application services
/////////////////////////////////////////////////////////////////
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// The default Swashbuckle handling of TimeSpan is incorrect for > .NET 6.0.2 because of a
// breaking change in System.Text.Json, see
// https://learn.microsoft.com/dotnet/core/compatibility/serialization/6.0/timespan-serialization-format
// The fix below is taken from a comment on Swashbuckle's related GitHub issue:
// https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2505#issuecomment-1279557743
builder.Services.AddSwaggerGen(
    options =>
    {
        options.MapType<TimeSpan>(
            () => new OpenApiSchema
            {
                Type = "string",
                Example = new OpenApiString("00:00:00"),
            });
    });

builder.Services.AddHealthChecks().AddOpenApiDocument();

// Ensure both flavours of JsonOptions are configured the same, see
// https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2293
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
    options =>
    {
        ConfigureJsonSerializerOptions(options.SerializerOptions);
    });

// Swashbuckle currently gets its JsonSerializer options from the MVC JsonOptions.
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(
    options =>
    {
        ConfigureJsonSerializerOptions(options.JsonSerializerOptions);
    });

var app = builder.Build();

/////////////////////////////////////////////////////////////////
// Configure the HTTP request pipeline.
/////////////////////////////////////////////////////////////////
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

var reportFormatter = new HealthCheckReportFormatter();

app.MapHealthChecks(
        "/healthz",
        new HealthCheckOptions
        {
            ResponseWriter = reportFormatter.WriteDetailedReport,
        })
    .WithOpenApi<HealthCheckReport>(
        metadata =>
        {
            metadata.OperationId = "GET_healthz";
            metadata.Summary = "Returns information about the health of the system";
        });
app.MapControllers();

app.Run();

void ConfigureJsonSerializerOptions(JsonSerializerOptions jsonSerializerOptions)
{
    jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
}
