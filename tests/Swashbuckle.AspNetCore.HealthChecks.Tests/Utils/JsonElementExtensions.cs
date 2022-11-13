using System.Text.Json;

namespace Swashbuckle.AspNetCore.HealthChecks.Tests.Utils;

public static class JsonElementExtensions
{
    public static JsonElementAssertions Should(this JsonElement instance)
    {
        return new JsonElementAssertions(instance);
    }

    public static bool HasValue(this JsonElement element, string value)
    {
        return element.ValueKind == JsonValueKind.String &&
            string.Equals(element.GetString(), value, StringComparison.Ordinal);
    }
}
