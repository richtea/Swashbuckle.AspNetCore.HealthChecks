using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Swashbuckle.AspNetCore.HealthChecks;

// Disable certain StyleCop rules because this documentation is intended for Swagger not DocFX.
#pragma warning disable SA1629 // Documentation text should end with a period
#pragma warning disable SA1623 // The property's documentation summary text should begin with standard text

/// <summary>
/// The result of a health check.
/// </summary>
public class HealthCheckReport
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HealthCheckReport" /> class.
    /// </summary>
    public HealthCheckReport()
    {
        Duration = TimeSpan.Zero;
        Status = HealthStatus.Healthy;
    }

    /// <summary>
    /// The path of the health check endpoint.
    /// </summary>
    /// <remarks>
    /// Having the endpoint name in the response considerably simplifies the parsing logic, which can
    /// help when configuring automated monitoring software that might be checking multiple endpoints.
    /// </remarks>
    public string Endpoint { get; init; } = string.Empty;

    /// <summary>
    /// The health check status, derived from the status of the <see cref="Checks"/>.
    /// </summary>
    /// <example>healthy</example>
    public HealthStatus Status { get; init; }

    /// <summary>
    /// The execution duration of the entire health check.
    /// </summary>
    /// <example>00:00:00.012345</example>
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// The individual checks that form part of the overall result.
    /// </summary>
    public ICollection<HealthCheckEntry> Checks { get; init; } = new List<HealthCheckEntry>();
}
