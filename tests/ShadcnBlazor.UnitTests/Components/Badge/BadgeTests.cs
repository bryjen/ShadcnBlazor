using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Components.Shared.Models.Enums;
using ShadcnBlazor.Tests.Shared.TestComponents.Badge;

namespace ShadcnBlazor.UnitTests.Components.Badge;

[TestFixture]
public class BadgeTests : BaseTest
{
    [Test]
    public void Badge_RendersDefault()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Badge.Badge>(p => p
            .AddChildContent("Test Badge"));

        // Assert
        var badge = cut.Find("[data-slot='badge']");
        badge.Should().NotBeNull();
        badge.TextContent.Should().Contain("Test Badge");
    }

    [TestCase(Variant.Default, "Default")]
    [TestCase(Variant.Secondary, "Secondary")]
    [TestCase(Variant.Destructive, "Destructive")]
    [TestCase(Variant.Outline, "Outline")]
    [TestCase(Variant.Ghost, "Ghost")]
    public void Badge_AllVariants_HaveCorrectDataAttribute(Variant variant, string expectedValue)
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Badge.Badge>(p => p
            .Add(x => x.Variant, variant)
            .AddChildContent("Variant Badge"));

        // Assert
        var badge = cut.Find("[data-slot='badge']");
        var variantAttr = badge.GetAttribute("data-variant");
        variantAttr.Should().Be(expectedValue);
    }

    [Test]
    public void BadgeLink_RendersAnchorTag()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Badge.BadgeLink>(p => p
            .Add(x => x.Href, "/test")
            .AddChildContent("Link Badge"));

        // Assert
        var link = cut.Find("a");
        link.Should().NotBeNull();
        link.GetAttribute("href").Should().Be("/test");
        link.TextContent.Should().Contain("Link Badge");
    }

    [Test]
    public void BadgeLink_SetsTargetAttribute()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Badge.BadgeLink>(p => p
            .Add(x => x.Href, "/test")
            .Add(x => x.Target, "_blank")
            .AddChildContent("External Link"));

        // Assert
        var link = cut.Find("a");
        link.GetAttribute("target").Should().Be("_blank");
    }

    [Test]
    public void Badge_ChildContent_IsRendered()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Badge.Badge>(p => p
            .AddChildContent("Hello World"));

        // Assert
        cut.Markup.Should().Contain("Hello World");
    }
}
