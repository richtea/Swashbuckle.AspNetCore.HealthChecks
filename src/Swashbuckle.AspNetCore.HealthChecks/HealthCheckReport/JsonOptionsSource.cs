using System.Text.Json;

namespace Swashbuckle.AspNetCore.HealthChecks;

/// <summary>
/// The source to use when resolving the <see cref="JsonSerializerOptions" /> that configure the output format of a
/// <see cref="HealthCheckReport"/>.
/// </summary>
/// <remarks>
/// <para>
/// There are two different classes that configure JSON processing options in .NET Core - one is
/// <see cref="Microsoft.AspNetCore.Mvc.JsonOptions"/> and the other is
/// <see cref="Microsoft.AspNetCore.Http.Json.JsonOptions"/>. Swashbuckle uses the MVC <c>JsonOptions</c> when
/// generating OpenAPI documents, presumably because the MVC controllers are configured by these settings. However,
/// minimal APIs use the variant from <c>Microsoft.AspNetCore.Http.Json</c>. The recommendation in the associated GitHub
/// issue (https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2293) is to ensure that both options are
/// configured identically.
/// </para>
/// <para>
/// This library supports either source for its <c>JsonSerializerOptions</c>, and this enumeration defines the
/// supported sources.
/// </para>
/// </remarks>
public enum JsonOptionsSource
{
    /// <summary>
    /// Derive serializer options from <see cref="Microsoft.AspNetCore.Http.Json.JsonOptions.SerializerOptions" />.
    /// </summary>
    HttpJsonOptions,

    /// <summary>
    /// Derive serializer options from <see cref="Microsoft.AspNetCore.Mvc.JsonOptions" />.
    /// </summary>
    MvcJsonOptions,
}
