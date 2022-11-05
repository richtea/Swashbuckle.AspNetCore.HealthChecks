using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.HealthChecks.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.HealthChecks;

/// <summary>
/// Performs post-configuration on <see cref="SwaggerGeneratorOptions"/>.
/// </summary>
public class SwaggerGeneratorOptionsPostConfiguration : IPostConfigureOptions<SwaggerGeneratorOptions>
{
    private readonly HealthCheckApiExplorerOptions _apiExplorerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="SwaggerGeneratorOptionsPostConfiguration"/> class.
    /// </summary>
    /// <param name="options">The <see cref="HealthCheckApiExplorerOptions"/> instance.</param>
    public SwaggerGeneratorOptionsPostConfiguration(IOptions<HealthCheckApiExplorerOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _apiExplorerOptions = options.Value;
    }

    /// <inheritdoc />
    public void PostConfigure(string name, SwaggerGeneratorOptions options)
    {
        if (_apiExplorerOptions.CreateHealthCheckOpenApiDocument)
        {
            if (string.IsNullOrWhiteSpace(_apiExplorerOptions.HealthCheckOpenApiDocumentName))
            {
                throw new InvalidOperationException(
                    "OpenApiDocumentName must be provided if CreateOpenApiDocument is specified");
            }

            options.SwaggerDocs.Add(_apiExplorerOptions.HealthCheckOpenApiDocumentName, _apiExplorerOptions.HealthCheckOpenApiDocumentInfo);
            var originalDocInclusionPredicate = options.DocInclusionPredicate;
            options.DocInclusionPredicate = (docName, apiDescription) =>
            {
                var isHealthCheck = apiDescription.ActionDescriptor is HealthCheckActionDescriptor;

                if (string.Equals(docName, _apiExplorerOptions.HealthCheckOpenApiDocumentName, StringComparison.Ordinal))
                {
                    return isHealthCheck;
                }

                return !isHealthCheck && originalDocInclusionPredicate(docName, apiDescription);
            };
        }
    }
}
