using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Tests.Shared.TestComponents.Checkbox;

namespace ShadcnBlazor.UnitTests.Components.Checkbox;

[TestFixture]
public class CheckboxTests : BaseTest
{
    [Test]
    public void Checkbox_DefaultState_IsUnchecked()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Checkbox.Checkbox>(p => p
            .AddChildContent("Default"));

        // Assert
        var input = cut.Find("button[role='checkbox']");
        var ariaChecked = input.GetAttribute("aria-checked");
        ariaChecked.Should().Be("false");
    }

    [Test]
    public void Checkbox_Checked_True_IsChecked()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Checkbox.Checkbox>(p => p
            .Add(x => x.Checked, true)
            .AddChildContent("Checked"));

        // Assert
        var input = cut.Find("button[role='checkbox']");
        var ariaChecked = input.GetAttribute("aria-checked");
        ariaChecked.Should().Be("true");
    }

    [Test]
    public void Checkbox_Disabled_PreventsInteraction()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Checkbox.Checkbox>(p => p
            .Add(x => x.Disabled, true)
            .AddChildContent("Disabled"));

        // Assert
        var input = cut.Find("button[role='checkbox']");
        input.HasAttribute("disabled").Should().BeTrue();
    }

    [Test]
    public void Checkbox_Invalid_SetsAriaInvalid()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Checkbox.Checkbox>(p => p
            .Add(x => x.Invalid, true)
            .AddChildContent("Invalid"));

        // Assert
        var input = cut.Find("button[role='checkbox']");
        var ariaInvalid = input.GetAttribute("aria-invalid");
        ariaInvalid.Should().Be("true");
    }

    [Test]
    public void Checkbox_ChildContent_IsRendered()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Checkbox.Checkbox>(p => p
            .AddChildContent("Label Text"));

        // Assert
        cut.Markup.Should().Contain("Label Text");
    }
}
