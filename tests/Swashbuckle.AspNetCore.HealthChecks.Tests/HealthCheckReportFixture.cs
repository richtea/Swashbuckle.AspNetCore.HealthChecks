using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Http.Json;
#endif
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.HealthChecks.Tests.Utils;

namespace Swashbuckle.AspNetCore.HealthChecks.Tests;

/// <summary>
/// Provides a set of tests to verify the functionality of health check reporting
/// and formatting within the application. This class contains various unit tests
/// for validating the behavior of health check endpoints, their output formats,
/// and response structure in different configurations and scenarios.
/// </summary>
public class HealthCheckReportFixture
{
    [Fact]
    public async Task basic_health_report()
    {
        var result = new HealthCheckResult(HealthStatus.Healthy, "test_description");
        var options = new HealthCheckReportFormatOptions();
        var host = await CreateTestHostBuilder(result, options).StartAsync();

        using var client = host.GetTestClient();
        using var response = await client.GetAsync("/healthz");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(contentStream);
        document.RootElement.Should().HaveElement("$.endpoint")
            .Which.Should().HaveValue("/healthz");
        document.RootElement.Should().HaveElement("$.status")
            .Which.Should().HaveValue("healthy");
        document.RootElement.Should().HaveElement("$.duration");
        document.RootElement.Should().HaveElement("$.checks")
            .Which.Should().BeAnArray().Which.Should().HaveCount(1);

        document.RootElement.Should().HaveElement("$.checks[0].name")
            .Which.Should().HaveValue("Sample");
        document.RootElement.Should().HaveElement("$.checks[0].description")
            .Which.Should().HaveValue("test_description");
        document.RootElement.Should().HaveElement("$.checks[0].duration");
        document.RootElement.Should().HaveElement("$.checks[0].status")
            .Which.Should().HaveValue("healthy");
        document.RootElement.Should().NotHaveElement("$.checks[0].data");
        document.RootElement.Should().NotHaveElement("$.checks[0].error");
    }

    [Fact]
    public async Task report_contains_data_when_available()
    {
        var data = new Dictionary<string, object>
        {
            { "Key1", "Value1" },
            { "Key2", 2 },
        };
        var result = new HealthCheckResult(HealthStatus.Healthy, "test_description", data: data);
        var options = new HealthCheckReportFormatOptions();
        var host = await CreateTestHostBuilder(result, options).StartAsync();

        using var client = host.GetTestClient();
        using var response = await client.GetAsync("/healthz");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(contentStream);
        document.RootElement.Should().HaveElement("$.checks[0].data").Which.Should().BeAnObject();
        document.RootElement.Should().HaveElement("$.checks[0].data.Key1").Which.Should().HaveValue("Value1");
        document.RootElement.Should().HaveElement("$.checks[0].data.Key2").Which.Should().HaveValue(2);
    }

    [Fact]
    public async Task uses_http_json_when_configured()
    {
        var result = new HealthCheckResult(HealthStatus.Healthy);
        var options = new HealthCheckReportFormatOptions { JsonOptionsSource = JsonOptionsSource.HttpJsonOptions };
        var host = await CreateTestHostBuilder(
                result,
                options,
                jsonOptions =>
                {
                    // Ensure naming policy isn't camelcase, and omit the JsonStringEnumConverter, so
                    // we can distinguish between the Http and Mvc JSON settings
                    jsonOptions.SerializerOptions.PropertyNamingPolicy = null;
                })
            .StartAsync();

        using var client = host.GetTestClient();
        using var response = await client.GetAsync("/healthz");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(contentStream);
        document.RootElement.Should().HaveElement("$.Status").Which.Should().HaveValue(2);
        document.RootElement.Should().HaveElement("$.Checks[0].Status").Which.Should().HaveValue(2);
    }

    [Fact]
    public async Task uses_mvc_json_when_configured()
    {
        var result = new HealthCheckResult(HealthStatus.Healthy);
        var options = new HealthCheckReportFormatOptions { JsonOptionsSource = JsonOptionsSource.MvcJsonOptions };
        var host = await CreateTestHostBuilder(
                result,
                options,
                jsonOptions =>
                {
                    // Ensure naming policy isn't camelcase, and omit the JsonStringEnumConverter, so
                    // we can distinguish between the Http and Mvc JSON settings
                    // Note that these options shouldn't be active, because we have specified the MVC JSON options above
                    jsonOptions.SerializerOptions.PropertyNamingPolicy = null;
                })
            .StartAsync();

        using var client = host.GetTestClient();
        using var response = await client.GetAsync("/healthz");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(contentStream);
        document.RootElement.Should().HaveElement("$.status").Which.Should().HaveValue("healthy");
        document.RootElement.Should().HaveElement("$.checks[0].status").Which.Should().HaveValue("healthy");
    }

    [Fact]
    public async Task report_contains_exception_when_available()
    {
        // Create an exception with a stack trace
        Exception? exception;
        try
        {
            throw new InvalidOperationException("You can't do that, like the Beatles said");
        }
        catch (Exception e)
        {
            exception = e;
        }

        var result = new HealthCheckResult(HealthStatus.Unhealthy, "test_description", exception);
        var options = new HealthCheckReportFormatOptions();
        var host = await CreateTestHostBuilder(result, options).StartAsync();

        using var client = host.GetTestClient();
        using var response = await client.GetAsync("/healthz");

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(contentStream);
        document.RootElement.Should().HaveElement("$.status")
            .Which.Should().HaveValue("unhealthy");
        document.RootElement.Should().HaveElement("$.checks[0].status")
            .Which.Should().HaveValue("unhealthy");
        document.RootElement.Should().HaveElement("$.checks[0].error")
            .Which.Should().HaveValue("You can't do that, like the Beatles said");
    }

    private static IHostBuilder CreateTestHostBuilder(
        HealthCheckResult healthCheckResult,
        HealthCheckReportFormatOptions? reportFormatOptions = null,
        Action<JsonOptions>? configureHttpJsonOptions = null,
        Action<Microsoft.AspNetCore.Mvc.JsonOptions>? configureMvcJsonOptions = null)
    {
        configureHttpJsonOptions ??= options =>
        {
            ConfigureDefaultJsonSerializerOptions(options.SerializerOptions);
        };

        configureMvcJsonOptions ??= options =>
        {
            ConfigureDefaultJsonSerializerOptions(options.JsonSerializerOptions);
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
                                services.Configure(configureHttpJsonOptions);
                                services.Configure(configureMvcJsonOptions);

                                services.AddControllers();
                                services.AddEndpointsApiExplorer();
                                services.AddSwaggerGen();
                                services.AddHealthChecks()
                                    .AddTypeActivatedCheck<TestHealthCheck>(
                                        "Sample",
                                        HealthStatus.Degraded,
                                        new[] { "sample" },
                                        healthCheckResult)
                                    .AddOpenApi(
                                        options =>
                                        {
                                            options.CreateHealthCheckOpenApiDocument = true;
                                        });
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
                                        ConfigureHealthCheckEndpoints(endpoints);
                                        endpoints.MapControllers();
                                    });
                            });
                });
        return host;

        void ConfigureHealthCheckEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks(
                    "/healthz",
                    new HealthCheckOptions
                    {
                        ResponseWriter = new HealthCheckReportFormatter(reportFormatOptions).WriteDetailedReport,
                    })
                .WithOpenApi<string>();
        }
    }

    private static void ConfigureDefaultJsonSerializerOptions(JsonSerializerOptions jsonSerializerOptions)
    {
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }
}
