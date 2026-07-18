using FluentAssertions;
using MobileShop.Common.Pagination;

namespace MobileShop.UnitTests.Common;

public class PageRequestTests
{
    [Fact]
    public void Page_DefaultsToOne()
    {
        new PageRequest().Page.Should().Be(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Page_ClampsToOne_WhenSetBelowOne(int input)
    {
        var request = new PageRequest { Page = input };
        request.Page.Should().Be(1);
    }

    [Fact]
    public void PageSize_DefaultsTo20()
    {
        new PageRequest().PageSize.Should().Be(20);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PageSize_FallsBackToDefault_WhenNonPositive(int input)
    {
        var request = new PageRequest { PageSize = input };
        request.PageSize.Should().Be(20);
    }

    [Fact]
    public void PageSize_ClampsToMax_WhenAboveLimit()
    {
        var request = new PageRequest { PageSize = 500 };
        request.PageSize.Should().Be(100);
    }
}
