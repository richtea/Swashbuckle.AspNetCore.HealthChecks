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

public class SwaggerTests
{
    [Fact]
    public async Task swagger_contains_default_values()
    {
        var host = await CreateTestHostBuilder(
                options =>
                {
                    options.CreateHealthCheckOpenApiDocument = true;
                },
                endpoints =>
                {
                    endpoints.MapHealthChecks("/healthz")
                        .WithOpenApi<string>();
                })
            .StartAsync();
        ;

        var client = host.GetTestClient();
        var response = await client.GetAsync("/swagger/health-checks/swagger.json");

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
    public async Task can_configure_document_info_properties()
    {
        var host = await CreateTestHostBuilder(
                options =>
                {
                    options.CreateHealthCheckOpenApiDocument = true;
                    options.HealthCheckOpenApiDocumentInfo.Title = "Test title";
                    options.HealthCheckOpenApiDocumentInfo.Version = "Test version";
                },
                endpoints =>
                {
                    endpoints.MapHealthChecks("/healthz")
                        .WithOpenApi<string>();
                })
            .StartAsync();

        var client = host.GetTestClient();
        var response = await client.GetAsync("/swagger/health-checks/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(contentStream);
        document.RootElement.Should().HaveElement("$.info.title")
            .Which.Should().HaveValue("Test title");
        document.RootElement.Should().HaveElement("$.info.version")
            .Which.Should().HaveValue("Test version");
    }

    [Fact]
    public async Task can_configure_summary_text()
    {
        var host = await CreateTestHostBuilder(
                options =>
                {
                    options.CreateHealthCheckOpenApiDocument = true;
                },
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

        var client = host.GetTestClient();
        var response = await client.GetAsync("/swagger/health-checks/swagger.json");

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
                options =>
                {
                    options.CreateHealthCheckOpenApiDocument = true;
                },
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

        var client = host.GetTestClient();
        var response = await client.GetAsync("/swagger/health-checks/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(contentStream);
        document.RootElement.Should().HaveElement("$.paths['/healthz'].get.description")
            .Which.Should().HaveValue("Test description");
    }

    private static IHostBuilder CreateTestHostBuilder(
        Action<HealthCheckApiExplorerOptions> configureOptions,
        Action<IEndpointRouteBuilder> configureEndpoints)
    {
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

                                services.AddEndpointsApiExplorer();
                                services.AddSwaggerGen();
                                services.AddHealthChecks().AddOpenApi(configureOptions);
                            })
                        .Configure(
                            app =>
                            {
                                app.UseSwagger();
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
