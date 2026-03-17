using Bunit;
using Microsoft.AspNetCore.Components;
using NUnit.Framework;
using ShadcnBlazor.Tests.Shared.Extensions;

namespace ShadcnBlazor.UnitTests;

[TestFixture]
public abstract class BaseTest
{
    private Bunit.TestContext? _testContext;

    protected Bunit.TestContext TestContext
    {
        get
        {
            if (_testContext == null)
            {
                _testContext = new Bunit.TestContext();
                // Configure JSInterop to be loose by default for headless tests
                _testContext.JSInterop.Mode = JSRuntimeMode.Loose;

                // Register shared services
                _testContext.Services.AddShadcnTestServices();
            }
            return _testContext;
        }
    }

    [TearDown]
    public void TearDown()
    {
        _testContext?.Dispose();
        _testContext = null;
    }
}
