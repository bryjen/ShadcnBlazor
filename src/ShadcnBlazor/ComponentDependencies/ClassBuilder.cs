using TailwindMerge;

namespace ShadcnBlazor.ComponentDependencies;

/// <summary>
/// Utility class for building CSS class strings with variant support, similar to class-variance-authority.
/// </summary>
public static class ClassBuilder
{
    private static readonly TwMerge _twMerge = new();

    /// <summary>
    /// Merges multiple class strings, removing duplicates and empty entries.
    /// </summary>
    public static string Merge(params string?[] classes)
    {
        // Filter out null/empty entries first to avoid unnecessary work.
        var nonEmpty = classes
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .ToArray();

        if (nonEmpty.Length == 0)
        {
            return string.Empty;
        }

        // Use TwMerge to perform Tailwind-aware conflict resolution.
        var safeNonEmpty = nonEmpty.Select(c => c!).ToArray();
        return _twMerge.Merge(safeNonEmpty);
    }

    /// <summary>
    /// Builds a class string from base classes and variant-specific classes.
    /// </summary>
    public static string Build(string baseClasses, Dictionary<string, string>? variants = null, string? additionalClasses = null)
    {
        var allClasses = new List<string>();

        if (!string.IsNullOrWhiteSpace(baseClasses))
        {
            allClasses.Add(baseClasses);
        }

        if (variants != null)
        {
            foreach (var variant in variants.Values)
            {
                if (!string.IsNullOrWhiteSpace(variant))
                {
                    allClasses.Add(variant);
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(additionalClasses))
        {
            allClasses.Add(additionalClasses);
        }

        return Merge(allClasses.ToArray());
    }
}
