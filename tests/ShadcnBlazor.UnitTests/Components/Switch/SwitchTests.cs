using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Components.Shared.Models.Enums;
using ShadcnBlazor.Tests.Shared.TestComponents.Switch;

namespace ShadcnBlazor.UnitTests.Components.Switch;

[TestFixture]
public class SwitchTests : BaseTest
{
    [Test]
    public void Switch_DefaultState_IsUnchecked()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Switch.Switch>(p => p
            .AddChildContent("Switch Item"));

        // Assert
        var button = cut.Find("button[role='switch']");
        button.GetAttribute("aria-checked").Should().Be("false");
        button.GetAttribute("data-state").Should().Be("unchecked");
    }

    [Test]
    public void Switch_Checked_True_IsChecked()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Switch.Switch>(p => p
            .Add(x => x.Checked, true)
            .AddChildContent("Switch Item"));

        // Assert
        var button = cut.Find("button[role='switch']");
        button.GetAttribute("aria-checked").Should().Be("true");
        button.GetAttribute("data-state").Should().Be("checked");
    }

    [TestCase(Size.Sm, "sm")]
    [TestCase(Size.Md, "md")]
    [TestCase(Size.Lg, "lg")]
    public void Switch_AllSizes_HaveCorrectDataAttribute(Size size, string expectedValue)
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Switch.Switch>(p => p
            .Add(x => x.Size, size));

        // Assert
        var button = cut.Find("button[role='switch']");
        button.GetAttribute("data-size").Should().Be(expectedValue);
    }

    [Test]
    public void Switch_Disabled_PreventsInteraction()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Switch.Switch>(p => p
            .Add(x => x.Disabled, true));

        // Assert
        var button = cut.Find("button[role='switch']");
        button.HasAttribute("disabled").Should().BeTrue();
    }

    [Test]
    public void Switch_Click_TogglesState()
    {
        // Arrange
        var val = false;
        var cut = TestContext.Render<ShadcnBlazor.Components.Switch.Switch>(p => p
            .Add(x => x.Checked, val)
            .Add(x => x.CheckedChanged, (bool res) => val = res));

        // Act
        var button = cut.Find("button[role='switch']");
        button.Click();

        // Assert
        val.Should().BeTrue();
    }

    [Test]
    public void Switch_ChildContent_IsRendered()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Switch.Switch>(p => p
            .AddChildContent("Toggle Me"));

        // Assert
        cut.Markup.Should().Contain("Toggle Me");
    }
}
