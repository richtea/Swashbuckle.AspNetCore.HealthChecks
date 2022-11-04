using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Swashbuckle.AspNetCore.HealthChecks.ApiExplorer;

/// <summary>
/// A custom <see cref="ActionDescriptor" /> specialization that supports health check endpoint 'actions'.
/// </summary>
internal sealed class HealthCheckActionDescriptor : ActionDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HealthCheckActionDescriptor" /> class.
    /// </summary>
    /// <param name="metadata">The metadata associated with the health check.</param>
    public HealthCheckActionDescriptor(HealthCheckDescriptionMetadata metadata)
    {
        Metadata = metadata;
        DisplayName = metadata.DisplayName;
    }

    public HealthCheckDescriptionMetadata Metadata { get; }
}
