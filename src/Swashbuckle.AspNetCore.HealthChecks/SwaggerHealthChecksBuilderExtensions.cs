using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.HealthChecks;
using Swashbuckle.AspNetCore.HealthChecks.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

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
    /// <param name="configureOptions">A callback to configure the <see cref="HealthCheckApiExplorerOptions"/>.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
    public static IHealthChecksBuilder AddOpenApi(this IHealthChecksBuilder builder, Action<HealthCheckApiExplorerOptions>? configureOptions = null)
    {
        ThrowHelper.ThrowIfNull(builder);

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Transient<IApiDescriptionProvider, HealthCheckApiDescriptionProvider>());

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Transient<IConfigureOptions<SwaggerGenOptions>, SwaggerGenOptionsConfiguration>());

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Transient<IPostConfigureOptions<SwaggerGeneratorOptions>, SwaggerGeneratorOptionsPostConfiguration>());

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Transient<IPostConfigureOptions<SwaggerUIOptions>, SwaggerUIOptionsPostConfiguration>());

        if (configureOptions != null)
        {
            builder.Services.ConfigureHealthCheckApiExplorer(configureOptions);
        }

        return builder;
    }

    /// <summary>
    /// Adds custom API descriptions support to the specified <see cref="IServiceCollection"/>, and creates an OpenAPI
    /// document.
    /// </summary>
    /// <param name="builder">The health checks builder.</param>
    /// <param name="configureOptions">A callback to configure the <see cref="HealthCheckApiExplorerOptions"/>.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
    public static IHealthChecksBuilder AddOpenApiDocument(
        this IHealthChecksBuilder builder,
        Action<HealthCheckApiExplorerOptions>? configureOptions = null)
    {
        void InnerConfigure(HealthCheckApiExplorerOptions options)
        {
            options.CreateHealthCheckOpenApiDocument = true;
            configureOptions?.Invoke(options);
        }

        return builder.AddOpenApi(InnerConfigure);
    }

    /// <summary>
    /// Configures ApiExplorer options for the health check endpoints.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">A callback to configure the <see cref="HealthCheckApiExplorerOptions"/>.</param>
    /// <returns>A reference to the service collection, for chaining.</returns>
    public static IServiceCollection ConfigureHealthCheckApiExplorer(
        this IServiceCollection services,
        Action<HealthCheckApiExplorerOptions> configureOptions)
    {
        ThrowHelper.ThrowIfNull(services);

        services.Configure(configureOptions);
        return services;
    }
}
