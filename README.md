# Swashbuckle.AspNetCore.HealthChecks

![CI build status](https://github.com/richtea/Swashbuckle.AspNetCore.HealthChecks/actions/workflows/ci.yml/badge.svg)
![GitHub latest release](https://img.shields.io/github/v/release/richtea/Swashbuckle.AspNetCore.HealthChecks?include_prereleases&sort=semver)
![Nuget](https://img.shields.io/nuget/v/Swashbuckle.AspNetCore.HealthChecks)

This library provides [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) support for [ASP.NET Core
health checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks).

## Basic usage

In your startup code, add OpenAPI support for health checks as follows:

```csharp
// Add an OpenAPI document that contains health check API definitions
builder.Services.AddHealthChecks().AddOpenApiDocument();

// Other setup...

// For each configured health check, configure the OpenAPI metadata
app.MapHealthChecks("/healthz").WithOpenApi<string>();
```

Use the delegate overload to specify additional metadata about the health check endpoint. These metadata are exposed in
the generated OpenAPI document:

```csharp
app.MapHealthChecks("/healthz")
    .WithOpenApi<string>(
        metadata =>
        {
            metadata.OperationId = "GET_healthz";
            metadata.Summary = "Returns information about the health of the system";
        });
```

## Detailed health check response

The default output from a health check endpoint is a plaintext response that contains the overall health check status,
e.g. `Degraded`. In the above examples, the plaintext response is indicated by the use of `string` as the type parameter
when calling the `WithOpenApi<string>` extension method.

If you want to return a more detailed response, the health check subsystem enables you to customize the output
as described [here](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks#customize-output).

This library includes a custom type, `HealthCheckReport`, that contains detailed information about the health check
result. To use this type, you need to configure the `HealthCheckOptions.ResponseWriter` as shown below.

```csharp
app.MapHealthChecks(
        "/healthz",
        new HealthCheckOptions
        {
            ResponseWriter = new HealthCheckReportFormatter().WriteDetailedReport,
        })
    .WithOpenApi<HealthCheckReport>(
        metadata =>
        {
            metadata.OperationId = "GET_healthz";
            metadata.Summary = "Returns information about the health of the system";
        });
```

## How it works

As described in its
[documentation](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#swashbuckle-apiexplorer-and-routing),
Swashbuckle generates OpenAPI documents with information that it derives from `ApiExplorer`, the API metadata layer that
ships with ASP.NET Core.

This library works by generating custom health check metadata for the `ApiExplorer` layer. In addition, it configures a
custom Swashbuckle [operation filter](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#operation-filters) that
maps the custom health check metadata into the OpenAPI document.

## Advanced usage

The default settings are designed to enable developers to get up and running as quickly as possible. The library also
supports more advanced scenarios.

### OpenAPI document

By default, the `AddOpenApiDocument()` extension method generates a separate OpenAPI document for health check
endpoints, available at `/swagger/health-checks/swagger.json`. However, this behavior is configurable.

#### Modifying the document name

To use the name `health` instead of `health-checks`, set the
`HealthCheckApiExplorerOptions.HealthCheckOpenApiDocumentName` property:

```csharp
builder.Services.AddHealthChecks()
    .AddOpenApiDocument(
        options =>
        {
            options.HealthCheckOpenApiDocumentName = "health";
        });
```

#### Modifying the document info properties

An OpenAPI document has several top-level properties that are exposed through the `HealthCheckOpenApiDocumentInfo`
property. You can modify these as you require:

```csharp
builder.Services.AddHealthChecks().AddOpenApiDocument(
    options =>
    {
        options.HealthCheckOpenApiDocumentInfo.Description = "Some description";
        options.HealthCheckOpenApiDocumentInfo.Version = "0.1";
    });
```

#### Suppressing the generation of a separate OpenAPI document

To suppress the default generation of a separate OpenAPI document, call the `AddOpenApi()` extension method instead of
`AddOpenApiDocument()`:

```csharp
builder.Services.AddHealthChecks().AddOpenApi();
```

The same effect can be achieved by setting the `HealthCheckApiExplorerOptions.CreateHealthCheckOpenApiDocument` property
to `false`.

## Credits

A huge thank you to all the contributors to Swashbuckle, especially
[domaindrivendev](https://github.com/domaindrivendev), who posted the original
[suggestion](https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1058#issuecomment-480789069) that inspired
me to create this library.
