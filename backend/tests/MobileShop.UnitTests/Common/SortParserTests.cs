using FluentAssertions;
using MobileShop.Common.Sorting;

namespace MobileShop.UnitTests.Common;

public class SortParserTests
{
    private static readonly Dictionary<string, string> AllowedFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["price"] = "Price",
        ["brand"] = "Brand",
    };

    [Theory]
    [InlineData("price_asc", "Price", false)]
    [InlineData("price_desc", "Price", true)]
    [InlineData("BRAND_ASC", "Brand", false)]
    public void Parse_ReturnsSortOption_ForAllowedField(string sort, string expectedField, bool expectedDescending)
    {
        var result = SortParser.Parse(sort, AllowedFields);

        result.Should().NotBeNull();
        result!.Value.Field.Should().Be(expectedField);
        result.Value.Descending.Should().Be(expectedDescending);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("createdat_asc")] // not in the whitelist — must not fall through to an arbitrary field
    [InlineData("price_sideways")] // no recognized direction suffix
    [InlineData("price")]
    public void Parse_ReturnsNull_ForInvalidOrDisallowedInput(string? sort)
    {
        var result = SortParser.Parse(sort, AllowedFields);
        result.Should().BeNull();
    }
}
