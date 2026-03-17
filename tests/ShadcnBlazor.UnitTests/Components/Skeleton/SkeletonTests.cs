using Bunit;
using FluentAssertions;
using NUnit.Framework;
using ShadcnBlazor.Tests.Shared.TestComponents.Skeleton;

namespace ShadcnBlazor.UnitTests.Components.Skeleton;

[TestFixture]
public class SkeletonTests : BaseTest
{
    [Test]
    public void Skeleton_Renders()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Skeleton.Skeleton>();

        // Assert
        var skeleton = cut.Find("div[class*='animate-pulse']");
        skeleton.Should().NotBeNull();
    }

    [Test]
    public void Skeleton_AdditionalClass_IsApplied()
    {
        // Act
        var cut = TestContext.Render<ShadcnBlazor.Components.Skeleton.Skeleton>(p => p
            .Add(x => x.Class, "h-12 w-12 rounded-full"));

        // Assert
        var skeleton = cut.Find("div");
        var classAttr = skeleton.GetAttribute("class");
        classAttr.Should().Contain("h-12");
        classAttr.Should().Contain("w-12");
    }
}
