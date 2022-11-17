using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Swashbuckle.AspNetCore.HealthChecks;

// Disable certain StyleCop rules because this documentation is intended for Swagger, where a different style applies.
// For example, "Gets or sets..." doesn't make sense
#pragma warning disable SA1629 // Documentation text should end with a period
#pragma warning disable SA1623 // The property's documentation summary text should begin with standard text

/// <summary>
/// Contains additional detail about a health check.
/// </summary>
public class HealthCheckEntry
{
    /// <summary>
    /// The name of the health check.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// The data associated with the health check.
    /// </summary>
    public IDictionary<string, object>? Data { get; init; }

    /// <summary>
    /// A description of the health check.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The execution duration of the individual health check.
    /// </summary>
    /// <example>00:00:00.012345</example>
    public TimeSpan Duration { get; init; } = TimeSpan.Zero;

    /// <summary>
    /// The health check status.
    /// </summary>
    /// <example>healthy</example>
    public HealthStatus Status { get; init; } = HealthStatus.Healthy;

    /// <summary>
    /// A description of the health check error, if any error occurred.
    /// </summary>
    public string? Error { get; init; }
}
