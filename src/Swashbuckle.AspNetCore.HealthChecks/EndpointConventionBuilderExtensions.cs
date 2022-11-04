using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.HealthChecks.ApiExplorer;

namespace Swashbuckle.AspNetCore.HealthChecks;

/// <summary>
/// Contains extension methods on <see cref="IEndpointConventionBuilder"/>.
/// </summary>
public static class EndpointConventionBuilderExtensions
{
    /// <summary>
    /// Adds an ApiExplorer description to a health check endpoint.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configureMetadata">A callback to configure the endpoint metadata.</param>
    /// <returns>The <see cref="IEndpointConventionBuilder"/>.</returns>
    public static IEndpointConventionBuilder WithApiDescription(
        this IEndpointConventionBuilder builder,
        Action<HealthCheckDescriptionMetadata>? configureMetadata = null)
    {
        var metadata = new HealthCheckDescriptionMetadata();
        metadata.ResponseDefinitions.Add(
            new HealthCheckResponseDefinition
            {
                StatusCode = StatusCodes.Status200OK,
                ResponseType = typeof(HealthReport),
            });
        metadata.ResponseDefinitions.Add(
            new HealthCheckResponseDefinition
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable,
                ResponseType = typeof(HealthReport),
            });

        configureMetadata?.Invoke(metadata);

        builder.WithMetadata(metadata);
        return builder;
    }
}
