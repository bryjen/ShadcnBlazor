using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Components.Shared.Models.Enums;
using ShadcnBlazor.Tests.Shared.TestComponents.ToggleButton;

namespace ShadcnBlazor.UnitTests.Components.ToggleButton;

[TestFixture]
public class ToggleButtonTests : BaseTest
{
    [Test]
    public void ToggleButton_DefaultState_IsUnpressed()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.ToggleButton.ToggleButton>(p => p
            .AddChildContent("Toggle Content"));

        // Assert
        var button = cut.Find("button[role='button']");
        button.GetAttribute("aria-pressed").Should().Be("false");
    }

    [Test]
    public void ToggleButton_Pressed_True_IsPressed()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.ToggleButton.ToggleButton>(p => p
            .Add(x => x.IsToggled, true)
            .AddChildContent("Toggle Content"));

        // Assert
        var button = cut.Find("button[role='button']");
        button.GetAttribute("aria-pressed").Should().Be("true");
    }

    [TestCase(Variant.Default)]
    [TestCase(Variant.Secondary)]
    [TestCase(Variant.Outline)]
    [TestCase(Variant.Ghost)]
    [TestCase(Variant.Link)]
    public void ToggleButton_AllVariants_RendersCorrectly(Variant variant)
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.ToggleButton.ToggleButton>(p => p
            .Add(x => x.VariantToggled, variant)
            .AddChildContent("Toggle Content"));

        // Assert
        var button = cut.Find("button[role='button']");
        button.Should().NotBeNull();
    }

    [Test]
    public void ToggleButton_Disabled_PreventsInteraction()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.ToggleButton.ToggleButton>(p => p
            .Add(x => x.Disabled, true));

        // Assert
        var button = cut.Find("button[role='button']");
        button.HasAttribute("disabled").Should().BeTrue();
    }

    [Test]
    public void ToggleButton_Click_TogglesState()
    {
        // Arrange
        var val = false;
        var cut = TestContext.Render<ShadcnBlazor.Components.ToggleButton.ToggleButton>(p => p
            .Add(x => x.IsToggled, val)
            .Add(x => x.IsToggledChanged, (bool res) => val = res));

        // Act
        var button = cut.Find("button[role='button']");
        button.Click();

        // Assert
        val.Should().BeTrue();
    }

    [Test]
    public void ToggleButton_ChildContent_IsRendered()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.ToggleButton.ToggleButton>(p => p
            .AddChildContent("Bold Text"));

        // Assert
        cut.Markup.Should().Contain("Bold Text");
    }
}
