using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Components.Alert;
using ShadcnBlazor.Tests.Shared.TestComponents.Alert;

namespace ShadcnBlazor.UnitTests.Components.Alert;

[TestFixture]
public class AlertTests : BaseTest
{
    [Test]
    public void Alert_RendersWithRole()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Alert.Alert>(p => p
            .AddChildContent("Alert content"));

        // Assert
        var alert = cut.Find("[role='alert']");
        alert.Should().NotBeNull();
    }

    [Test]
    public void Alert_DefaultVariant_HasDataSlot()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Alert.Alert>(p => p
            .Add(x => x.Variant, AlertVariant.Default)
            .AddChildContent("Default"));

        // Assert
        var alert = cut.Find("[data-slot='alert']");
        alert.Should().NotBeNull();
    }

    [Test]
    public void Alert_DestructiveVariant_Renders()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Alert.Alert>(p => p
            .Add(x => x.Variant, AlertVariant.Destructive)
            .AddChildContent("Destructive"));

        // Assert
        cut.Markup.Should().Contain("Destructive");
    }

    [Test]
    public void Alert_ChildContent_IsRendered()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Alert.Alert>(p => p
            .AddChildContent("Alert Message"));

        // Assert
        cut.Markup.Should().Contain("Alert Message");
    }
}
