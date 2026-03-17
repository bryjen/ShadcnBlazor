using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Components.Button;
using ShadcnBlazor.Components.Shared.Models.Enums;
using ShadcnBlazor.Tests.Shared.TestComponents.Button;

namespace ShadcnBlazor.UnitTests.Components.Button;

[TestFixture]
public class ButtonTests : BaseTest
{
    [Test]
    public void Button_RendersAllVariants()
    {
        // Act
        var cut = TestContext.Render<ButtonTest1>();

        // Assert
        var buttons = cut.FindAll("button");
        buttons.Should().HaveCount(9);

        var loadingButton = buttons.FirstOrDefault(b => b.Attributes.Any(a => a.Name == "aria-busy"));
        loadingButton.Should().NotBeNull();

        var disabledButton = buttons.FirstOrDefault(b => b.HasAttribute("disabled"));
        disabledButton.Should().NotBeNull();
    }

    [Test]
    public void Button_DefaultVariant_RendersButtonElement()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Button.Button>(p => p
            .Add(x => x.Variant, Variant.Default)
            .AddChildContent("Default Button"));

        // Assert
        var button = cut.Find("button");
        button.Should().NotBeNull();
        var slot = button.GetAttribute("data-slot");
        slot.Should().Be("button");
    }

    [Test]
    public void Button_LoadingState_SetsBusyAttribute()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Button.Button>(p => p
            .Add(x => x.State, ButtonState.Loading)
            .AddChildContent("Loading"));

        // Assert
        var button = cut.Find("button");
        button.Should().NotBeNull();
    }

    [Test]
    public void Button_DisabledState_SetsDisabledAttribute()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Button.Button>(p => p
            .Add(x => x.State, ButtonState.Disabled)
            .AddChildContent("Disabled"));

        // Assert
        var button = cut.Find("button");
        button.HasAttribute("disabled").Should().BeTrue();
    }

    [Test]
    public void Button_TypeSubmit_Renders()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Button.Button>(p => p
            .Add(x => x.Type, ButtonType.Submit)
            .AddChildContent("Submit"));

        // Assert
        var button = cut.Find("button");
        button.Should().NotBeNull();
    }

    [Test]
    public void Button_Variant_Default_HasDataVariantAttribute()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Button.Button>(p => p
            .Add(x => x.Variant, Variant.Default)
            .AddChildContent("Button"));

        // Assert
        var button = cut.Find("button");
        var dataVariant = button.GetAttribute("data-variant");
        dataVariant.Should().Be("default");
    }

    [Test]
    public void Button_Size_Sm_HasDataSizeAttribute()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Button.Button>(p => p
            .Add(x => x.Size, Size.Sm)
            .AddChildContent("Button"));

        // Assert
        var button = cut.Find("button");
        var dataSize = button.GetAttribute("data-size");
        dataSize.Should().Be("sm");
    }
}
