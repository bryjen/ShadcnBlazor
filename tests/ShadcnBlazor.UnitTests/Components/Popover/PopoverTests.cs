using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Components.Popover;
using ShadcnBlazor.Tests.Shared.TestComponents.Popover;

namespace ShadcnBlazor.UnitTests.Components.Popover;

[TestFixture]
public class PopoverTests : BaseTest
{
    [Test]
    public void Popover_RendersAnchor()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Popover.Popover>(p => p
            .Add(x => x.Anchor, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "id", "popover-trigger");
                builder.AddContent(2, "Open Popover");
                builder.CloseElement();
            }))
            .Add(x => x.ChildContent, "Popover Content"));

        // Assert
        cut.Find("#popover-trigger").Should().NotBeNull();
        cut.Find("#popover-trigger").TextContent.Should().Be("Open Popover");
    }

    [Test]
    public void Popover_OpenState_ReflectedInAttributes()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Popover.Popover>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Anchor, (Microsoft.AspNetCore.Components.RenderFragment)(builder => builder.AddContent(0, "Anchor")))
            .Add(x => x.ChildContent, "Content"));

        // Assert
        var anchor = cut.Find("div.popover-anchor");
        anchor.GetAttribute("aria-expanded").Should().Be("true");
    }

    [Test]
    public void Popover_ClosedState_ReflectedInAttributes()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Popover.Popover>(p => p
            .Add(x => x.Open, false)
            .Add(x => x.Anchor, (Microsoft.AspNetCore.Components.RenderFragment)(builder => builder.AddContent(0, "Anchor")))
            .Add(x => x.ChildContent, "Content"));

        // Assert
        var anchor = cut.Find("div.popover-anchor");
        anchor.GetAttribute("aria-expanded").Should().Be("false");
    }
}
