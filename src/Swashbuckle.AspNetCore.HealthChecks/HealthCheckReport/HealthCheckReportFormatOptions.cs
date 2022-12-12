using System.Text.Json;

namespace Swashbuckle.AspNetCore.HealthChecks;

/// <summary>
/// The options that control the behavior of a <see cref="HealthCheckReportFormatter" />.
/// </summary>
/// <param name="JsonOptionsSource">
/// The source to use when resolving <see cref="JsonSerializerOptions" />.
/// </param>
#if !NET5_0_OR_GREATER
// ReSharper disable once NotAccessedPositionalProperty.Global
#endif
public record HealthCheckReportFormatOptions(JsonOptionsSource JsonOptionsSource = JsonOptionsSource.MvcJsonOptions);
