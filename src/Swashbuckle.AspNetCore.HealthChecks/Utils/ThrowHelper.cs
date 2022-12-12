using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Swashbuckle.AspNetCore.HealthChecks;

/// <summary>
/// Helper to throw exceptions, we can't use `ArgumentNullException.ThrowIfNull()` as we are multi-targeting.
/// </summary>
/// <remarks>
/// See https://andrewlock.net/exploring-dotnet-6-part-11-callerargumentexpression-and-throw-helpers/.
/// </remarks>
public static class ThrowHelper
{
    /// <summary>Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.</summary>
    /// <param name="argument">The pointer argument to validate as non-null.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    public static void ThrowIfNull(
        [NotNull] object? argument,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        if (argument is null)
        {
            Throw(paramName);
        }
    }

    [DoesNotReturn]
    private static void Throw(string? paramName)
    {
        throw new ArgumentNullException(paramName);
    }
}
