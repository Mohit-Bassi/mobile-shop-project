namespace MobileShop.Common.Sorting;

public readonly record struct SortOption(string Field, bool Descending);

public static class SortParser
{
    /// <summary>
    /// Parses a "field_asc" / "field_desc" query value against a whitelist of allowed field names
    /// (case-insensitive keys mapping to their canonical form). Returns null if the input is empty
    /// or not on the whitelist — callers should fall back to a default sort in that case.
    /// </summary>
    public static SortOption? Parse(string? sort, IReadOnlyDictionary<string, string> allowedFields)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return null;
        }

        var descending = sort.EndsWith("_desc", StringComparison.OrdinalIgnoreCase);
        var ascending = sort.EndsWith("_asc", StringComparison.OrdinalIgnoreCase);

        if (!descending && !ascending)
        {
            return null;
        }

        var fieldKey = sort[..sort.LastIndexOf('_')];

        return allowedFields.TryGetValue(fieldKey, out var canonicalField)
            ? new SortOption(canonicalField, descending)
            : null;
    }
}
