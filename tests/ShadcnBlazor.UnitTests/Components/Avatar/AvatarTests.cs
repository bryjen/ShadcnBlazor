using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Components.Shared.Models.Enums;
using ShadcnBlazor.Tests.Shared.TestComponents.Avatar;

namespace ShadcnBlazor.UnitTests.Components.Avatar;

[TestFixture]
public class AvatarTests : BaseTest
{
    [Test]
    public void Avatar_RendersImage_WhenSrcProvided()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Avatar.Avatar>(p => p
            .Add(x => x.Src, "test.png")
            .Add(x => x.Alt, "User Name"));

        // Assert
        var img = cut.Find("img");
        img.GetAttribute("src").Should().Be("test.png");
        img.GetAttribute("alt").Should().Be("User Name");
    }

    [Test]
    public void Avatar_RendersFallbackText_FromAlt_WhenNoSrc()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Avatar.Avatar>(p => p
            .Add(x => x.Alt, "John Doe"));

        // Assert
        cut.Markup.Should().Contain("JD");
    }

    [Test]
    public void Avatar_RendersChildContent_WhenNoSrcAndNoAlt()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Avatar.Avatar>(p => p
            .AddChildContent("FB"));

        // Assert
        cut.Markup.Should().Contain("FB");
    }

    [TestCase(Size.Sm, "size-6")]
    [TestCase(Size.Md, "size-8")]
    [TestCase(Size.Lg, "size-10")]
    public void Avatar_AllSizes_HaveCorrectClass(Size size, string expectedClass)
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Avatar.Avatar>(p => p
            .Add(x => x.Size, size));

        // Assert
        var avatar = cut.Find("[data-slot='avatar']");
        avatar.GetAttribute("class").Should().Contain(expectedClass);
    }

    [Test]
    public void Avatar_RendersBadge()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Avatar.Avatar>(p => p
            .Add(x => x.Badge, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "id", "test-badge");
                builder.CloseElement();
            })));

        // Assert
        cut.Find("#test-badge").Should().NotBeNull();
    }
}
