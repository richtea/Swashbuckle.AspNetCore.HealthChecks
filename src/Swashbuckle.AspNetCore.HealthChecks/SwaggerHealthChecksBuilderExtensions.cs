using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.HealthChecks;
using Swashbuckle.AspNetCore.HealthChecks.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Contains extension methods that enable Swagger support for health checks.
/// </summary>
public static class SwaggerHealthChecksBuilderExtensions
{
    /// <summary>
    /// Adds custom API descriptions support to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="builder">The health checks builder.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
    public static IHealthChecksBuilder AddSwagger(this IHealthChecksBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Transient<IApiDescriptionProvider, HealthCheckApiDescriptionProvider>());

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Transient<IConfigureOptions<SwaggerGenOptions>, SwaggerGenOptionsConfiguration>());

        return builder;
    }
}
