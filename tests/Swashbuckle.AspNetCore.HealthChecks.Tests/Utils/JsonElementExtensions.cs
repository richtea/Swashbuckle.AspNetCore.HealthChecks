using System.Text.Json;

namespace Swashbuckle.AspNetCore.HealthChecks.Tests.Utils;

public static class JsonElementExtensions
{
    public static JsonElementAssertions Should(this JsonElement instance)
    {
        return new JsonElementAssertions(instance);
    }
}
