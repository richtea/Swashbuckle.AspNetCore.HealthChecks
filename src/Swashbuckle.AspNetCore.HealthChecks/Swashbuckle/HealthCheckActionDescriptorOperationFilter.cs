using System.Diagnostics.CodeAnalysis;
#if NET10_0_OR_GREATER
using Microsoft.OpenApi;
#else
using Microsoft.OpenApi.Models;
#endif
using Swashbuckle.AspNetCore.HealthChecks.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.HealthChecks;

/// <summary>
/// An <see cref="IOperationFilter" /> to enhance health check OpenAPI documentation based on the contents of the
/// associated <see cref="HealthCheckActionDescriptor"/> metadata, if present.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Instantiated implicitly")]
internal class HealthCheckActionDescriptorOperationFilter : IOperationFilter
{
    /// <summary>
    /// Applies the filter to the specified operation.
    /// </summary>
    /// <param name="operation">The OpenAPI operation being processed.</param>
    /// <param name="context">The filter context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor is not HealthCheckActionDescriptor actionDescriptor)
        {
            return;
        }

        operation.Summary = actionDescriptor.Metadata.Summary;
        operation.Description = actionDescriptor.Metadata.Description;
        operation.OperationId = actionDescriptor.Metadata.OperationId;
    }
}
