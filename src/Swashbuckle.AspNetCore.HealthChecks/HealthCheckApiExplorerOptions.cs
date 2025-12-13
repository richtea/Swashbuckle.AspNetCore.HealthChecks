#if NET10_0_OR_GREATER
using Microsoft.OpenApi;
#else
using Microsoft.OpenApi.Models;
#endif
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.HealthChecks;

/// <summary>
/// Defines the options that control how the extension registers health checks with ApiExplorer.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class HealthCheckApiExplorerOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to create an OpenAPI document for health checks.
    /// </summary>
    /// <remarks>
    /// This option is designed to work with the default Swashbuckle configuration, which produces a single OpenAPI
    /// document. By setting this to <c>true</c>, a separate OpenAPI document is defined for health check endpoints,
    /// and various other settings are affected so that the endpoints can be extracted.
    /// If you have configured multiple OpenAPI documents, you may need to set this to <c>false</c>
    /// and set up the documents and the <see cref="SwaggerGeneratorOptions.DocInclusionPredicate"/> manually.
    /// </remarks>
    public bool CreateHealthCheckOpenApiDocument { get; set; }

    /// <summary>
    /// Gets or sets the name of the OpenAPI document that contains health checks.
    /// </summary>
    /// <remarks>
    /// This value is ignored if <see cref="CreateHealthCheckOpenApiDocument"/> is <c>false</c>.
    /// </remarks>
    public string HealthCheckOpenApiDocumentName { get; set; } = "health-checks";

    /// <summary>
    /// Gets or sets the <see cref="OpenApiInfo"/> that describes the OpenAPI document that contains health checks.
    /// </summary>
    /// <remarks>
    /// This value is ignored if <see cref="CreateHealthCheckOpenApiDocument"/> is <c>false</c>.
    /// </remarks>
    public OpenApiInfo HealthCheckOpenApiDocumentInfo { get; set; } = new() { Title = "Health Checks", Version = "1.0" };
}
