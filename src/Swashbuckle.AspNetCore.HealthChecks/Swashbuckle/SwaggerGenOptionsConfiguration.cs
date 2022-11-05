using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.HealthChecks.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.HealthChecks;

/// <summary>
/// Adds the necessary configuration to <see cref="SwaggerGenOptions"/> to support health checks.
/// </summary>
internal class SwaggerGenOptionsConfiguration : IConfigureOptions<SwaggerGenOptions>
{
    private readonly HealthCheckApiExplorerOptions _apiExplorerOptions;

    public SwaggerGenOptionsConfiguration(IOptions<HealthCheckApiExplorerOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _apiExplorerOptions = options.Value;
    }

    /// <summary>
    /// Invoked to configure a <see cref="SwaggerGenOptions"/> instance.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(SwaggerGenOptions options)
    {
        options.OperationFilter<HealthCheckActionDescriptorOperationFilter>();

        // Swashbuckle's SwaggerGeneratorOptions.DefaultTagsSelector throws an exception if the "controller" key
        // is not present in the action's route values. Obviously this endpoint is not a controller, so that key
        // is missing. To prevent the exception, we therefore use a custom selector (below).
        var originalTagsSelector = options.SwaggerGeneratorOptions.TagsSelector;
        options.SwaggerGeneratorOptions.TagsSelector = description =>
        {
            if (description.ActionDescriptor is not HealthCheckActionDescriptor descriptor)
            {
                return originalTagsSelector(description);
            }

            // SwaggerUI uses the tag to group operations - if none present, we need to provide a default value
            return descriptor.Metadata.Tags.Any() ? descriptor.Metadata.Tags : new[] { "HealthChecks" };
        };

        // TODO: check we need this
        // Prevent performance problem when returning standard HealthReport
        // See https://stackoverflow.com/a/67826291/260213
        options.MapType<Exception>(() => new OpenApiSchema { Type = "object" });

        if (_apiExplorerOptions.CreateHealthCheckOpenApiDocument)
        {
            if (string.IsNullOrWhiteSpace(_apiExplorerOptions.HealthCheckOpenApiDocumentName))
            {
                throw new InvalidOperationException(
                    "OpenApiDocumentName must be provided if CreateOpenApiDocument is specified");
            }
        }
    }
}
