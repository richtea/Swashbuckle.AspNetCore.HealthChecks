using System.Text.Json;

namespace Swashbuckle.AspNetCore.HealthChecks;

/// <summary>
/// The options that control the behavior of a <see cref="HealthCheckReportFormatter" />.
/// </summary>
/// <param name="JsonOptionsSource">
/// The source to use when resolving <see cref="JsonSerializerOptions" />.
/// </param>
public record HealthCheckReportFormatOptions(JsonOptionsSource JsonOptionsSource = JsonOptionsSource.MvcJsonOptions);
