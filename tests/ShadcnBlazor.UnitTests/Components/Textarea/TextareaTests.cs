using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Components.Shared.Models.Enums;
using ShadcnBlazor.Tests.Shared.TestComponents.Textarea;

namespace ShadcnBlazor.UnitTests.Components.Textarea;

[TestFixture]
public class TextareaTests : BaseTest
{
    [Test]
    public void Textarea_RendersDefault()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Textarea.Textarea>(p => p
            .Add(x => x.Placeholder, "Enter message"));

        // Assert
        var textarea = cut.Find("textarea");
        textarea.GetAttribute("placeholder").Should().Be("Enter message");
    }

    [Test]
    public void Textarea_Rows_Applied()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Textarea.Textarea>(p => p
            .Add(x => x.Rows, 5));

        // Assert
        var textarea = cut.Find("textarea");
        textarea.GetAttribute("rows").Should().Be("5");
    }

    [Test]
    public void Textarea_Value_IsApplied()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Textarea.Textarea>(p => p
            .Add(x => x.Value, "Sample Text"));

        // Assert
        var textarea = cut.Find("textarea");
        textarea.GetAttribute("value").Should().Be("Sample Text");
    }

    [Test]
    public void Textarea_OnChange_TriggersCallback()
    {
        // Arrange
        string? val = "";
        var cut = TestContext.Render<ShadcnBlazor.Components.Textarea.Textarea>(p => p
            .Add(x => x.Value, val)
            .Add(x => x.ValueChanged, (string? res) => val = res));

        // Act
        var textarea = cut.Find("textarea");
        textarea.Change("New Content");

        // Assert
        val.Should().Be("New Content");
    }

    [Test]
    public void Textarea_Disabled_PreventsInteraction()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Textarea.Textarea>(p => p
            .Add(x => x.Disabled, true));

        // Assert
        var textarea = cut.Find("textarea");
        textarea.HasAttribute("disabled").Should().BeTrue();
    }
}
