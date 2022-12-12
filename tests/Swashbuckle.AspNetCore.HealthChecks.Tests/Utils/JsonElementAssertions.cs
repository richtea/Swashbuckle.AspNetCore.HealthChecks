using System.Text.Json;
using System.Text.Json.JsonDiffPatch;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Json.Path;

namespace Swashbuckle.AspNetCore.HealthChecks.Tests.Utils;

public class JsonElementAssertions : ReferenceTypeAssertions<JsonElement, JsonElementAssertions>
{
    public JsonElementAssertions(JsonElement instance)
        : base(instance)
    {
    }

    /// <inheritdoc />
    protected override string Identifier => "JsonElement";

    public AndConstraint<JsonElementAssertions> HaveValue(
        string expectedValue,
        StringComparison stringComparison = StringComparison.Ordinal,
        string because = "",
        params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.ValueKind == JsonValueKind.String)
            .FailWith("Element value is not a string")
            .Then
            .Given(() => Subject.GetString())
            .ForCondition(actualValue => string.Equals(actualValue, expectedValue, stringComparison))
            .FailWith(
                "Expected {context:JsonElement} to be {0}{reason}, but found {1}.",
                _ => expectedValue,
                actual => actual);

        return new AndConstraint<JsonElementAssertions>(this);
    }

    public AndConstraint<JsonElementAssertions> HaveValue(
        int expectedValue,
        string because = "",
        params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.ValueKind == JsonValueKind.Number)
            .FailWith("Element value is not a number")
            .Then
            .Given(() => Subject.GetInt64())
            .ForCondition(actualValue => actualValue == expectedValue)
            .FailWith(
                "Expected {context:JsonElement} to be {0}{reason}, but found {1}.",
                _ => expectedValue,
                actualValue => actualValue);

        return new AndConstraint<JsonElementAssertions>(this);
    }

    public AndConstraint<JsonElementAssertions> HaveValue(
        bool expectedValue,
        string because = "",
        params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.ValueKind is JsonValueKind.False or JsonValueKind.True)
            .FailWith("Element value is not a boolean")
            .Then
            .Given(() => Subject.GetBoolean())
            .ForCondition(actualValue => actualValue == expectedValue)
            .FailWith(
                "Expected {context:JsonElement} to be {0}{reason}, but found {1}.",
                _ => expectedValue,
                actualValue => actualValue);

        return new AndConstraint<JsonElementAssertions>(this);
    }

    public AndConstraint<JsonElementAssertions> BeEquivalentTo(
        JsonElement expectedValue,
        string because = "",
        params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.DeepEquals(expectedValue, JsonElementComparison.Semantic))
            .FailWith(
                "Expected {0} to equal {1}, but they are different",
                () => JsonSerializer.Serialize(Subject),
                () => JsonSerializer.Serialize(expectedValue));

        return new AndConstraint<JsonElementAssertions>(this);
    }

    public AndWhichConstraint<JsonElementAssertions, IEnumerable<JsonElement>> BeAnArray(
        string because = "",
        params object[] becauseArgs)
    {
        var success = Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.ValueKind is JsonValueKind.Array)
            .FailWith(
                "Expected {0} to be an array{reason}, but it was a {1}",
                () => JsonSerializer.Serialize(Subject),
                () => Subject.ValueKind);

        var arrayElements = success ? Subject.EnumerateArray().ToArray() : Enumerable.Empty<JsonElement>();
        return new AndWhichConstraint<JsonElementAssertions, IEnumerable<JsonElement>>(this, arrayElements);
    }

    public AndConstraint<JsonElementAssertions> BeAnObject(
        string because = "",
        params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.ValueKind is JsonValueKind.Object)
            .FailWith(
                "Expected {0} to be an object{reason}, but it was a {1}",
                () => JsonSerializer.Serialize(Subject),
                () => Subject.ValueKind);

        return new AndConstraint<JsonElementAssertions>(this);
    }

    public AndWhichConstraint<JsonElementAssertions, IEnumerable<JsonElement>> HaveElements(
        string jsonPath,
        OccurrenceConstraint occurrenceConstraint,
        string because = "",
        params object[] becauseArgs)
    {
        ThrowHelper.ThrowIfNull(jsonPath);

        var path = JsonPath.Parse(jsonPath);

        var results = path.Evaluate(Subject);

        IEnumerable<PathMatch> matched;

        var success = Execute.Assertion
            .ForCondition(results.Error == null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Path evaluation failed: {0}", results.Error!);

        if (success)
        {
            var actual = results.Matches!.Count;
            Execute.Assertion
                .ForConstraint(occurrenceConstraint, actual)
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    $"Expected {{context:JsonElement}} to contain path {{0}} {{expectedOccurrence}}{{reason}}, but found it {actual.Times()}",
                    jsonPath);

            matched = results.Matches;
        }
        else
        {
            matched = Enumerable.Empty<PathMatch>();
        }

        return new AndWhichConstraint<JsonElementAssertions, IEnumerable<JsonElement>>(
            this,
            matched.Select(m => m.Value));
    }

    public AndWhichConstraint<JsonElementAssertions, JsonElement> HaveElement(
        string jsonPath,
        string because = "",
        params object[] becauseArgs)
    {
        ThrowHelper.ThrowIfNull(jsonPath);

        var path = JsonPath.Parse(jsonPath);

        var results = path.Evaluate(Subject);

        JsonElement? element = null;

        var success = Execute.Assertion
            .ForCondition(results.Error == null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Path evaluation failed: {0}", results.Error!);

        if (success)
        {
            var actual = results.Matches!.Count;
            Execute.Assertion
                .ForCondition(actual > 0)
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    "Expected {context:JsonElement} to contain path {0}{reason}, but it was not found",
                    jsonPath);

            Execute.Assertion
                .ForCondition(actual < 2)
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    "Expected {context:JsonElement} to contain a single element at path {0}{reason}, but found {1} elements",
                    jsonPath,
                    actual);

            if (actual == 1)
            {
                element = results.Matches.Single().Value;
            }
        }

        return new AndWhichConstraint<JsonElementAssertions, JsonElement>(this, (JsonElement)element!);
    }

    public AndConstraint<JsonElementAssertions> NotHaveElement(
        string jsonPath,
        string because = "",
        params object[] becauseArgs)
    {
        ThrowHelper.ThrowIfNull(jsonPath);

        var path = JsonPath.Parse(jsonPath);

        var results = path.Evaluate(Subject);

        var success = Execute.Assertion
            .ForCondition(results.Error == null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Path evaluation failed: {0}", results.Error!);

        if (success)
        {
            var actual = results.Matches!.Count;
            Execute.Assertion
                .ForCondition(actual == 0)
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    "Expected {context:JsonElement} not to contain path {0}{reason}, but it was present",
                    jsonPath);
        }

        return new AndConstraint<JsonElementAssertions>(this);
    }
}
