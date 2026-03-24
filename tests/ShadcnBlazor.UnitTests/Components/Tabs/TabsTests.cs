using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Components.Tabs;
using ShadcnBlazor.Components.Shared.Models.Enums;
using ShadcnBlazor.Tests.Shared.TestComponents.Tabs;

namespace ShadcnBlazor.UnitTests.Components.Tabs;

[TestFixture]
public class TabsTests : BaseTest
{
    [Test]
    public void Tabs_SwitchContent_OnClick()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Tabs.Tabs>(p => p
            .Add(x => x.DefaultValue, "tab-1")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<TabsList>(0);
                builder.AddAttribute(1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                {
                    b.OpenComponent<TabsTrigger>(2);
                    b.AddAttribute(3, "Value", "tab-1");
                    b.AddAttribute(4, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b2 => b2.AddContent(5, "Trigger 1")));
                    b.CloseComponent();
                    b.OpenComponent<TabsTrigger>(6);
                    b.AddAttribute(7, "Value", "tab-2");
                    b.AddAttribute(8, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b2 => b2.AddContent(9, "Trigger 2")));
                    b.CloseComponent();
                }));
                builder.CloseComponent();

                builder.OpenComponent<TabsContent>(10);
                builder.AddAttribute(11, "Value", "tab-1");
                builder.AddAttribute(12, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(13, "Content 1")));
                builder.CloseComponent();
                builder.OpenComponent<TabsContent>(14);
                builder.AddAttribute(15, "Value", "tab-2");
                builder.AddAttribute(16, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(17, "Content 2")));
                builder.CloseComponent();
            }));

        // Assert - initially tab 1 open
        cut.Markup.Should().Contain("Content 1");
        cut.Markup.Should().NotContain("Content 2");

        // Act - Click tab 2 trigger
        var triggers = cut.FindAll("button");
        triggers[1].Click();

        // Assert - tab 2 open, tab 1 closed
        cut.Markup.Should().Contain("Content 2");
        cut.Markup.Should().NotContain("Content 1");
    }

    [Test]
    public void Tabs_VerticalOrientation_AppliesClass()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Tabs.Tabs>(p => p
            .Add(x => x.Orientation, "vertical"));

        // Assert
        var tabs = cut.Find("[data-slot='tabs']");
        tabs.GetAttribute("data-orientation").Should().Be("vertical");
    }
}
