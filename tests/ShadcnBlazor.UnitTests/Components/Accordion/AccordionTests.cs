using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Components.Accordion;
using ShadcnBlazor.Tests.Shared.TestComponents.Accordion;

namespace ShadcnBlazor.UnitTests.Components.Accordion;

[TestFixture]
public class AccordionTests : BaseTest
{
    [Test]
    public void Accordion_Single_TogglesItems()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Accordion.Accordion>(p => p
            .Add(x => x.Type, AccordionType.Single)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Value", "item-1");
                builder.AddAttribute(2, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                {
                    b.OpenComponent<AccordionTrigger>(3);
                    b.AddAttribute(4, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b2 => b2.AddContent(5, "Trigger 1")));
                    b.CloseComponent();
                    b.OpenComponent<AccordionContent>(6);
                    b.AddAttribute(7, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b2 => b2.AddContent(8, "Content 1")));
                    b.CloseComponent();
                }));
                builder.CloseComponent();

                builder.OpenComponent<AccordionItem>(9);
                builder.AddAttribute(10, "Value", "item-2");
                builder.AddAttribute(11, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                {
                    b.OpenComponent<AccordionTrigger>(12);
                    b.AddAttribute(13, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b2 => b2.AddContent(14, "Trigger 2")));
                    b.CloseComponent();
                    b.OpenComponent<AccordionContent>(15);
                    b.AddAttribute(16, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b2 => b2.AddContent(17, "Content 2")));
                    b.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert - initially closed
        cut.FindAll("button[data-slot='accordion-trigger'][data-state='open']").Should().BeEmpty();

        // Act - Click first trigger
        var trigger1 = cut.FindAll("button")[0];
        trigger1.Click();

        // Assert - item 1 open
        cut.Find("button[data-slot='accordion-trigger'][data-state='open']").Should().NotBeNull();
        cut.Markup.Should().Contain("Content 1");

        // Act - Click second trigger
        var trigger2 = cut.FindAll("button")[1];
        trigger2.Click();

        // Assert - item 2 open, item 1 closed (single mode)
        var openTriggers = cut.FindAll("button[data-slot='accordion-trigger'][data-state='open']");
        openTriggers.Count.Should().Be(1);
        cut.Markup.Should().Contain("Content 2");
        cut.Markup.Should().NotContain("Content 1");
    }

    [Test]
    public void Accordion_Multiple_AllowsMultipleOpen()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Accordion.Accordion>(p => p
            .Add(x => x.Type, AccordionType.Multiple)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Value", "item-1");
                builder.AddAttribute(2, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                {
                    b.OpenComponent<AccordionTrigger>(3);
                    b.AddAttribute(4, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b2 => b2.AddContent(5, "Trigger 1")));
                    b.CloseComponent();
                    b.OpenComponent<AccordionContent>(6);
                    b.AddAttribute(7, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b2 => b2.AddContent(8, "Content 1")));
                    b.CloseComponent();
                }));
                builder.CloseComponent();

                builder.OpenComponent<AccordionItem>(9);
                builder.AddAttribute(10, "Value", "item-2");
                builder.AddAttribute(11, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                {
                    b.OpenComponent<AccordionTrigger>(12);
                    b.AddAttribute(13, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b2 => b2.AddContent(14, "Trigger 2")));
                    b.CloseComponent();
                    b.OpenComponent<AccordionContent>(15);
                    b.AddAttribute(16, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b2 => b2.AddContent(17, "Content 2")));
                    b.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Act - Click both
        var triggers = cut.FindAll("button");
        triggers[0].Click();
        triggers[1].Click();

        // Assert
        var openTriggers = cut.FindAll("button[data-slot='accordion-trigger'][data-state='open']");
        openTriggers.Count.Should().Be(2);
        cut.Markup.Should().Contain("Content 1");
        cut.Markup.Should().Contain("Content 2");
    }
}
