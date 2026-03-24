using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Components.Shared.Models.Enums;
using ShadcnBlazor.Tests.Shared.TestComponents.Slider;

namespace ShadcnBlazor.UnitTests.Components.Slider;

[TestFixture]
public class SliderTests : BaseTest
{
    [Test]
    public void Slider_RendersDefault()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Slider.Slider>(p => p
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 100)
            .Add(x => x.Value, 50));

        // Assert
        var input = cut.Find("input[type='range']");
        input.GetAttribute("min").Should().Be("0");
        input.GetAttribute("max").Should().Be("100");
        input.GetAttribute("value").Should().Be("50");
    }

    [Test]
    public void Slider_Step_IsApplied()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Slider.Slider>(p => p
            .Add(x => x.Step, 5));

        // Assert
        var input = cut.Find("input[type='range']");
        input.GetAttribute("step").Should().Be("5");
    }

    [Test]
    public void Slider_OnInput_TriggersCallback()
    {
        // Arrange
        var val = 0.0;
        var cut = TestContext.Render<ShadcnBlazor.Components.Slider.Slider>(p => p
            .Add(x => x.Value, val)
            .Add(x => x.ValueChanged, (double res) => val = res));

        // Act
        var input = cut.Find("input[type='range']");
        input.Input("75");

        // Assert
        val.Should().Be(75);
    }

    [Test]
    public void RangeSlider_RendersTwoInputs()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Slider.RangeSlider>(p => p
            .Add(x => x.LowerValue, 20)
            .Add(x => x.UpperValue, 80));

        // Assert
        var inputs = cut.FindAll("input[type='range']");
        inputs.Count.Should().Be(2);
        inputs[0].GetAttribute("value").Should().Be("20");
        inputs[1].GetAttribute("value").Should().Be("80");
    }
}
