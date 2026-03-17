using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Components.Card;
using ShadcnBlazor.Tests.Shared.TestComponents.Card;
using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.UnitTests.Components.Card;

[TestFixture]
public class CardTests : BaseTest
{
    [Test]
    public void Card_Renders_WithDataSlot()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Card.Card>(p => p
            .AddChildContent("Card content"));

        // Assert
        var card = cut.Find("[data-slot='card']");
        card.Should().NotBeNull();
    }

    [Test]
    public void Card_Header_IsRendered()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Card.Card>(p => p
            .Add(x => x.Header, new RenderFragment(builder => builder.AddContent(0, "Header Text")))
            .AddChildContent("Content"));

        // Assert
        cut.Markup.Should().Contain("Header Text");
        cut.Find("[data-slot='card-header']").Should().NotBeNull();
    }

    [Test]
    public void Card_Footer_IsRendered()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Card.Card>(p => p
            .Add(x => x.Footer, new RenderFragment(builder => builder.AddContent(0, "Footer Text")))
            .AddChildContent("Content"));

        // Assert
        cut.Markup.Should().Contain("Footer Text");
        cut.Find("[data-slot='card-footer']").Should().NotBeNull();
    }

    [Test]
    public void Card_OutlineVariant_Renders()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Card.Card>(p => p
            .Add(x => x.Variant, CardVariant.Outline)
            .AddChildContent("Content"));

        // Assert
        cut.Markup.Should().Contain("Content");
    }

    [Test]
    public void Card_SmSize_Renders()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Card.Card>(p => p
            .Add(x => x.Size, CardSize.Sm)
            .AddChildContent("Content"));

        // Assert
        cut.Markup.Should().Contain("Content");
    }
}
