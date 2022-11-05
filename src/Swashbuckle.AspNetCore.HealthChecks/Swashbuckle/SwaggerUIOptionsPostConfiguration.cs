using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Swashbuckle.AspNetCore.HealthChecks;

/// <summary>
/// Performs post-configuration on the <see cref="SwaggerUIOptions" />.
/// </summary>
public class SwaggerUIOptionsPostConfiguration : IPostConfigureOptions<SwaggerUIOptions>
{
    private readonly HealthCheckApiExplorerOptions _apiExplorerOptions;

    private readonly IWebHostEnvironment _hostingEnv;

    /// <summary>
    /// Initializes a new instance of the <see cref="SwaggerUIOptionsPostConfiguration" /> class.
    /// </summary>
    /// <param name="apiExplorerOptions">The options instance that configures ApiExplorer for health checks.</param>
    /// <param name="hostingEnv">The Web application hosting environment.</param>
    public SwaggerUIOptionsPostConfiguration(
        IOptions<HealthCheckApiExplorerOptions> apiExplorerOptions,
        IWebHostEnvironment hostingEnv)
    {
        ArgumentNullException.ThrowIfNull(apiExplorerOptions);
        ArgumentNullException.ThrowIfNull(hostingEnv);

        _apiExplorerOptions = apiExplorerOptions.Value;
        _hostingEnv = hostingEnv;
    }

    /// <inheritdoc />
    public void PostConfigure(string name, SwaggerUIOptions options)
    {
        if (_apiExplorerOptions.CreateHealthCheckOpenApiDocument)
        {
            var urls = new List<UrlDescriptor>(options.ConfigObject.Urls ?? Enumerable.Empty<UrlDescriptor>());

            // This logic is duplicated from SwaggerUIBuilderExtensions.UseSwaggerUI(). It adds a default UI document
            // if no documents are currently defined
            if (options.ConfigObject.Urls == null)
            {
                urls.Add(
                    new UrlDescriptor
                    {
                        Url = "v1/swagger.json",
                        Name = $"{_hostingEnv.ApplicationName} v1",
                    });
            }

            // This adds a UI document for the health checks OpenAPI document
            urls.Add(
                new UrlDescriptor
                {
                    Url = $"{_apiExplorerOptions.HealthCheckOpenApiDocumentName}/swagger.json",
                    Name = "Health Checks",
                });

            options.ConfigObject.Urls = urls;
        }
    }
}
