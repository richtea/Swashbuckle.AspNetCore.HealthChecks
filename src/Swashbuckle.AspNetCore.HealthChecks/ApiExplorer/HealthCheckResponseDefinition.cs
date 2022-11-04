using System.Net.Mime;
using Microsoft.AspNetCore.Http;

namespace Swashbuckle.AspNetCore.HealthChecks.ApiExplorer;

/// <summary>
/// Contains information about the response produced by a health check endpoint.
/// </summary>
public class HealthCheckResponseDefinition
{
    /// <summary>
    /// Gets or sets the HTTP status code of the response.
    /// </summary>
    public int StatusCode { get; set; } = StatusCodes.Status200OK;

    /// <summary>
    /// Gets or sets the type that is returned in the response content.
    /// </summary>
    public Type? ResponseType { get; set; }

    /// <summary>
    /// Gets the media (MIME) types that are supported in the response.
    /// </summary>
    public IList<string> SupportedMediaTypes { get; } = new List<string> { MediaTypeNames.Application.Json };
}
