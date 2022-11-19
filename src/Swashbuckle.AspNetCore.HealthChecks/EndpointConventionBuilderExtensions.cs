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
    /// <param name="builder">The endpoint builder.</param>
    /// <param name="responseType">The <see cref="Type"/> that is returned by the health check endpoint.</param>
    /// <param name="configureMetadata">A callback to configure the endpoint metadata.</param>
    /// <returns>The <see cref="IEndpointConventionBuilder"/>.</returns>
    public static IEndpointConventionBuilder WithOpenApi(
        this IEndpointConventionBuilder builder,
        Type? responseType = null,
        Action<HealthCheckDescriptionMetadata>? configureMetadata = null)
    {
        var metadata = new HealthCheckDescriptionMetadata();
        metadata.ResponseDefinitions.Add(
            new HealthCheckResponseDefinition
            {
                StatusCode = StatusCodes.Status200OK,
                ResponseType = responseType,
            });
        metadata.ResponseDefinitions.Add(
            new HealthCheckResponseDefinition
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable,
                ResponseType = responseType,
            });

        configureMetadata?.Invoke(metadata);

        builder.WithMetadata(metadata);
        return builder;
    }

    /// <summary>
    /// Adds an ApiExplorer description to a health check endpoint.
    /// </summary>
    /// <typeparam name="TReport">The type of the result that is returned by the health check endpoint.</typeparam>
    /// <param name="builder">The endpoint builder.</param>
    /// <param name="configureMetadata">A callback to configure the endpoint metadata.</param>
    /// <returns>The <see cref="IEndpointConventionBuilder"/>.</returns>
    public static IEndpointConventionBuilder WithOpenApi<TReport>(
        this IEndpointConventionBuilder builder,
        Action<HealthCheckDescriptionMetadata>? configureMetadata = null)
    {
        return builder.WithOpenApi(typeof(TReport), configureMetadata);
    }
}
