using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Components.Shared.Models.Enums;
using ShadcnBlazor.Tests.Shared.TestComponents.Radio;

namespace ShadcnBlazor.UnitTests.Components.Radio;

[TestFixture]
public class RadioTests : BaseTest
{
    [Test]
    public void Radio_DefaultState_IsUnchecked()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Radio.Radio>(p => p
            .AddChildContent("Option 1"));

        // Assert
        var button = cut.Find("button[role='radio']");
        button.GetAttribute("aria-checked").Should().Be("false");
        button.GetAttribute("data-state").Should().Be("unchecked");
    }

    [Test]
    public void RadioGroup_Selects_CorrectItem()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Radio.RadioGroup>(p => p
            .Add(x => x.Value, "A")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ShadcnBlazor.Components.Radio.Radio>(0);
                builder.AddAttribute(1, "Value", "A");
                builder.AddAttribute(2, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(3, "Option A")));
                builder.CloseComponent();
                builder.OpenComponent<ShadcnBlazor.Components.Radio.Radio>(4);
                builder.AddAttribute(5, "Value", "B");
                builder.AddAttribute(6, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(7, "Option B")));
                builder.CloseComponent();
            }));

        // Assert
        var buttons = cut.FindAll("button[role='radio']");
        buttons[0].GetAttribute("aria-checked").Should().Be("true");
        buttons[1].GetAttribute("aria-checked").Should().Be("false");
    }

    [Test]
    public void Radio_Disabled_PreventsInteraction()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Radio.Radio>(p => p
            .Add(x => x.Disabled, true));

        // Assert
        var button = cut.Find("button[role='radio']");
        button.HasAttribute("disabled").Should().BeTrue();
    }

    [Test]
    public void Radio_Invalid_SetsDataInvalid()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Radio.Radio>(p => p
            .Add(x => x.Invalid, true));

        // Assert
        var label = cut.Find("label");
        label.GetAttribute("data-invalid").Should().Be("true");
    }
}
