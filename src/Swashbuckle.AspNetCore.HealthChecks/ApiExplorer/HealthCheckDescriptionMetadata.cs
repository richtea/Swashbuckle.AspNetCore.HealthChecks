namespace Swashbuckle.AspNetCore.HealthChecks.ApiExplorer;

/// <summary>
/// Contains metadata that describes a health check endpoint.
/// </summary>
public class HealthCheckDescriptionMetadata
{
    /// <summary>
    /// Gets a collection of objects that describe the responses that this endpoint produces.
    /// </summary>
    public IList<HealthCheckResponseDefinition> ResponseDefinitions { get; } =
        new List<HealthCheckResponseDefinition>();

    /// <summary>
    /// Gets or sets the OpenAPI group name to which this endpoint belongs.
    /// </summary>
    public string? GroupName { get; set; }

    /// <summary>
    /// Gets or sets the OpenAPI summary for the endpoint.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Gets or sets the OpenAPI description of the health check endpoint.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the OpenAPI OperationId of the health check endpoint.
    /// </summary>
    public string? OperationId { get; set; }

    /// <summary>
    /// Gets the OpenAPI tags that are associated with this endpoint.
    /// </summary>
    public IList<string> Tags { get; } = new List<string>();

    /// <summary>
    /// Gets or sets a friendly name for the ApiExplorer action descriptor.
    /// </summary>
    public string? DisplayName { get; set; }
}
