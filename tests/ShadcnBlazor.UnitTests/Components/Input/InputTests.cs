using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Components.Shared.Models.Enums;
using ShadcnBlazor.Tests.Shared.TestComponents.Input;

namespace ShadcnBlazor.UnitTests.Components.Input;

[TestFixture]
public class InputTests : BaseTest
{
    [Test]
    public void Input_RendersDefault()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Input.Input>(p => p
            .Add(x => x.Placeholder, "Enter name"));

        // Assert
        var input = cut.Find("input");
        input.GetAttribute("placeholder").Should().Be("Enter name");
        input.GetAttribute("type").Should().Be("text");
    }

    [TestCase("password")]
    [TestCase("email")]
    [TestCase("number")]
    public void Input_DifferentTypes_RendersCorrectly(string type)
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Input.Input>(p => p
            .Add(x => x.Type, type));

        // Assert
        var input = cut.Find("input");
        input.GetAttribute("type").Should().Be(type);
    }

    [TestCase(Size.Sm, "sm")]
    [TestCase(Size.Md, "md")]
    [TestCase(Size.Lg, "lg")]
    public void Input_AllSizes_HaveCorrectDataAttribute(Size size, string expectedValue)
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Input.Input>(p => p
            .Add(x => x.Size, size));

        // Assert
        var input = cut.Find("input");
        input.GetAttribute("data-size").Should().Be(expectedValue);
    }

    [Test]
    public void Input_Value_IsApplied()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Input.Input>(p => p
            .Add(x => x.Value, "Initial Value"));

        // Assert
        var input = cut.Find("input");
        input.GetAttribute("value").Should().Be("Initial Value");
    }

    [Test]
    public void Input_OnInput_TriggersCallback()
    {
        // Arrange
        var val = "";
        var cut = TestContext.Render<ShadcnBlazor.Components.Input.Input>(p => p
            .Add(x => x.Value, val)
            .Add(x => x.ValueChanged, (string res) => val = res));

        // Act
        var input = cut.Find("input");
        input.Input("New Value");

        // Assert
        val.Should().Be("New Value");
    }

    [Test]
    public void Input_Disabled_PreventsInteraction()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Input.Input>(p => p
            .Add(x => x.Disabled, true));

        // Assert
        var input = cut.Find("input");
        input.HasAttribute("disabled").Should().BeTrue();
    }
}
