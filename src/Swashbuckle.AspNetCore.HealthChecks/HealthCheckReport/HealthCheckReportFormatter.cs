using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Swashbuckle.AspNetCore.HealthChecks;

/// <summary>
/// Formats the results of a health check as a detailed health check response.
/// </summary>
public class HealthCheckReportFormatter
{
    private readonly HealthCheckReportFormatOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthCheckReportFormatter" /> class.
    /// </summary>
    /// <param name="options">The options that configure the response writer, or <c>null</c> to use the default options.</param>
    public HealthCheckReportFormatter(HealthCheckReportFormatOptions? options = null)
    {
        _options = options ?? new HealthCheckReportFormatOptions();
    }

    /// <summary>
    /// Writes the response for the specified report.
    /// </summary>
    /// <param name="context">The HTTP context into which to write the current health check report.</param>
    /// <param name="report">The health check report to be formatted.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task WriteDetailedReport(HttpContext context, HealthReport report)
    {
        var endpointPath = context.Request.Path.Value ?? string.Empty;

        var jsonSerializerOptions = _options.JsonOptionsSource switch
        {
            JsonOptionsSource.HttpJsonOptions => context.RequestServices
                .GetRequiredService<IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>>()
                .Value.SerializerOptions,
            JsonOptionsSource.MvcJsonOptions => context.RequestServices
                .GetRequiredService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>()
                .Value.JsonSerializerOptions,
            _ => throw new NotSupportedException("The specified value is not supported"),
        };

        var result = JsonSerializer.Serialize(
            new HealthCheckReport
            {
                Endpoint = endpointPath,
                Status = report.Status,
                Duration = report.TotalDuration,
                Checks = report.Entries.Select(
                    e => new HealthCheckEntry
                    {
                        Name = e.Key,
                        Description = e.Value.Description,
                        Duration = e.Value.Duration,
                        Status = e.Value.Status,
                        Error = e.Value.Exception?.Message,
                        Data = e.Value.Data.Count == 0 ? null : (IDictionary<string, object>)e.Value.Data,
                    }).ToList(),
            },
            jsonSerializerOptions);
        context.Response.ContentType = MediaTypeNames.Application.Json;
        return context.Response.WriteAsync(result, context.RequestAborted);
    }
}
