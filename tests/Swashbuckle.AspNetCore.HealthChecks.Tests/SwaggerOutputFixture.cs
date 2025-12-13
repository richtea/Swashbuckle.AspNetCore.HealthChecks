using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.HealthChecks.Tests.Utils;

namespace Swashbuckle.AspNetCore.HealthChecks.Tests;

/// <summary>
/// Provides test fixtures for validating Swagger/OpenAPI document output.
/// This class defines a series of test cases to ensure that Swagger documents are generated correctly
/// with default values, customized configurations, document properties, and endpoint descriptions.
/// </summary>
public class SwaggerOutputFixture
{
    [Fact]
    public async Task swagger_contains_default_values()
    {
        var host = await CreateTestHostBuilder().StartAsync();

        using var client = host.GetTestClient();
        using var response = await client.GetAsync("/swagger/health-checks/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(contentStream);

        var expectedFileName = Path.Join(
            AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
            "TestSupportFiles/standard-config.json");
        await using var stream = File.OpenRead(expectedFileName);
        using var expectedDoc = await JsonDocument.ParseAsync(stream);
        document.RootElement.Should().BeEquivalentTo(expectedDoc.RootElement);
    }

    [Fact]
    public async Task can_modify_default_openapi_document()
    {
        var host = await CreateTestHostBuilder(
                options =>
                {
                    options.CreateHealthCheckOpenApiDocument = false;
                })
            .StartAsync();

        using var client = host.GetTestClient();
        using var response = await client.GetAsync("/swagger/v1/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(contentStream);
        document.RootElement.Should().HaveElement("$.paths['/healthz'].get");
        document.RootElement.Should().HaveElement("$.paths['/WeatherForecast'].get");
    }

    [Fact]
    public async Task can_configure_document_info_properties()
    {
        var host = await CreateTestHostBuilder(
                options =>
                {
                    options.CreateHealthCheckOpenApiDocument = true;
                    options.HealthCheckOpenApiDocumentInfo.Title = "Test title";
                    options.HealthCheckOpenApiDocumentInfo.Version = "Test version";
                })
            .StartAsync();

        using var client = host.GetTestClient();
        using var response = await client.GetAsync("/swagger/health-checks/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(contentStream);
        document.RootElement.Should().HaveElement("$.info.title")
            .Which.Should().HaveValue("Test title");
        document.RootElement.Should().HaveElement("$.info.version")
            .Which.Should().HaveValue("Test version");
    }

    [Fact]
    public async Task can_configure_document_name()
    {
        var host = await CreateTestHostBuilder(
                options =>
                {
                    options.CreateHealthCheckOpenApiDocument = true;
                    options.HealthCheckOpenApiDocumentName = "test-doc-name";
                })
            .StartAsync();

        using var client = host.GetTestClient();
        using var response = await client.GetAsync("/swagger/test-doc-name/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task can_configure_summary_text()
    {
        var host = await CreateTestHostBuilder(
                null,
                endpoints =>
                {
                    endpoints.MapHealthChecks("/healthz")
                        .WithOpenApi<string>(
                            metadata =>
                            {
                                metadata.Summary = "Returns information about the health of the system";
                            });
                })
            .StartAsync();

        using var client = host.GetTestClient();
        using var response = await client.GetAsync("/swagger/health-checks/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(contentStream);
        document.RootElement.Should().HaveElement("$.paths['/healthz'].get.summary")
            .Which.Should().HaveValue("Returns information about the health of the system");
    }

    [Fact]
    public async Task can_configure_description()
    {
        var host = await CreateTestHostBuilder(
                null,
                endpoints =>
                {
                    endpoints.MapHealthChecks("/healthz")
                        .WithOpenApi<string>(
                            metadata =>
                            {
                                metadata.Description = "Test description";
                            });
                })
            .StartAsync();

        using var client = host.GetTestClient();
        using var response = await client.GetAsync("/swagger/health-checks/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(contentStream);
        document.RootElement.Should().HaveElement("$.paths['/healthz'].get.description")
            .Which.Should().HaveValue("Test description");
    }

    [Fact]
    public async Task can_set_operation_id()
    {
        var host = await CreateTestHostBuilder(
                null,
                endpoints =>
                {
                    endpoints.MapHealthChecks("/healthz")
                        .WithOpenApi<string>(
                            metadata =>
                            {
                                metadata.OperationId = "GET_HEALTHZ_CHECK";
                            });
                })
            .StartAsync();

        using var client = host.GetTestClient();
        using var response = await client.GetAsync("/swagger/health-checks/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(contentStream);
        document.RootElement.Should().HaveElement("$.paths['/healthz'].get.operationId")
            .Which.Should().HaveValue("GET_HEALTHZ_CHECK");
    }

    [Fact]
    public async Task can_set_tags()
    {
        var host = await CreateTestHostBuilder(
                null,
                endpoints =>
                {
                    endpoints.MapHealthChecks("/healthz")
                        .WithOpenApi<string>(
                            metadata =>
                            {
                                metadata.Tags.Add("test_tag1");
                                metadata.Tags.Add("test_tag2");
                            });
                })
            .StartAsync();

        using var client = host.GetTestClient();
        using var response = await client.GetAsync("/swagger/health-checks/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(contentStream);
        document.RootElement.Should().HaveElement("$.paths['/healthz'].get.tags")
            .Which.Should().BeAnArray()
            .Which.Should().HaveCount(2)
            .And.Contain(e => e.HasValue("test_tag1"))
            .And.Contain(e => e.HasValue("test_tag2"));
    }

    private static IHostBuilder CreateTestHostBuilder(
        Action<HealthCheckApiExplorerOptions>? configureOptions = null,
        Action<IEndpointRouteBuilder>? configureEndpoints = null)
    {
        configureOptions ??= options =>
        {
            options.CreateHealthCheckOpenApiDocument = true;
        };

        configureEndpoints ??= endpoints =>
        {
            endpoints.MapHealthChecks("/healthz").WithOpenApi<string>();
        };

        var host = new HostBuilder()
            .ConfigureWebHost(
                webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(
                            services =>
                            {
                                // Add services to the container.
                                services.AddControllers();

#if NET5_0_OR_GREATER
                                services.AddEndpointsApiExplorer();
#endif
                                services.AddSwaggerGen();
                                services.AddHealthChecks().AddOpenApi(configureOptions);
                            })
                        .Configure(
                            app =>
                            {
                                app.UseSwagger();
                                app.UseSwaggerUI();
                                app.UseRouting();
                                app.UseEndpoints(
                                    endpoints =>
                                    {
                                        configureEndpoints(endpoints);
                                        endpoints.MapControllers();
                                    });
                            });
                });
        return host;
    }
}
