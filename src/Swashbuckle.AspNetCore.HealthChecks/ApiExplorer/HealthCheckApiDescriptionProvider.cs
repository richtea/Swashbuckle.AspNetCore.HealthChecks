using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace Swashbuckle.AspNetCore.HealthChecks.ApiExplorer;

/// <summary>
/// An <see cref="IApiDescriptionProvider" /> that provides API descriptions for health check endpoints.
/// </summary>
internal class HealthCheckApiDescriptionProvider : IApiDescriptionProvider
{
    private readonly IModelMetadataProvider _modelMetadataProvider;

    private readonly EndpointDataSource _endpointDataSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthCheckApiDescriptionProvider" /> class.
    /// </summary>
    /// <param name="modelMetadataProvider">The model metadata provider.</param>
    /// <param name="endpointDataSource">The endpoint data source.</param>
    public HealthCheckApiDescriptionProvider(
        IModelMetadataProvider modelMetadataProvider,
        EndpointDataSource endpointDataSource)
    {
        _modelMetadataProvider =
            modelMetadataProvider ?? throw new ArgumentNullException(nameof(modelMetadataProvider));
        _endpointDataSource = endpointDataSource ?? throw new ArgumentNullException(nameof(endpointDataSource));
    }

    /// <inheritdoc />
    public int Order => 9999;

    /// <inheritdoc />
    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        foreach (var endpoint in _endpointDataSource.Endpoints)
        {
            var metadata = endpoint.Metadata.OfType<HealthCheckDescriptionMetadata>().FirstOrDefault();
            if (metadata is not null && endpoint is RouteEndpoint { RoutePattern: { } } re)
            {
                var apiDescription = CreateApiDescription(re, metadata);
                context.Results.Add(apiDescription);
            }
        }
    }

    /// <inheritdoc />
    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
    }

    private static string GetRelativePath(RouteEndpoint endpoint)
    {
        return string.Join('/', endpoint.RoutePattern.PathSegments.Select(GetLiteralSegmentText));
    }

    private static string GetLiteralSegmentText(RoutePatternPathSegment segment)
    {
        return string.Join(
            '/',
            segment.Parts.Where(p => p.IsLiteral).Select(p => ((RoutePatternLiteralPart)p).Content));
    }

    private ApiDescription CreateApiDescription(RouteEndpoint endpoint, HealthCheckDescriptionMetadata metadata)
    {
        var r = endpoint.RoutePattern;

        if (r.Parameters.Count > 0)
        {
            throw new NotSupportedException("Route parameters are not supported in health check endpoints");
        }

        var path = GetRelativePath(endpoint);

        var apiDescription = new ApiDescription
        {
            ActionDescriptor = new HealthCheckActionDescriptor(metadata),
            GroupName = metadata.GroupName,
            HttpMethod = "GET",
            RelativePath = path,
        };

        foreach (var apiResponse in metadata.ResponseDefinitions)
        {
            var responseType = new ApiResponseType { StatusCode = apiResponse.StatusCode };

            if (apiResponse.ResponseType != null)
            {
                responseType.Type = apiResponse.ResponseType;
                responseType.ModelMetadata = _modelMetadataProvider.GetMetadataForType(apiResponse.ResponseType);
            }

            foreach (var mediaType in apiResponse.SupportedMediaTypes)
            {
                responseType.ApiResponseFormats.Add(
                    new ApiResponseFormat { MediaType = mediaType });
            }

            apiDescription.SupportedResponseTypes.Add(responseType);
        }

        return apiDescription;
    }
}
