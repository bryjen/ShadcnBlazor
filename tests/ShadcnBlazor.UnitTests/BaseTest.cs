using Bunit;
using Microsoft.AspNetCore.Components;
using NUnit.Framework;
using ShadcnBlazor.Tests.Shared.Extensions;

namespace ShadcnBlazor.UnitTests;

[TestFixture]
public abstract class BaseTest
{
    private Bunit.BunitContext? _testContext;

    protected Bunit.BunitContext TestContext
    {
        get
        {
            if (_testContext == null)
            {
                _testContext = new Bunit.BunitContext();
                // Configure JSInterop to be loose by default for headless tests
                _testContext.JSInterop.Mode = JSRuntimeMode.Loose;

                // Register shared services
                _testContext.Services.AddShadcnTestServices();
            }
            return _testContext;
        }
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_testContext is not null)
        {
            await _testContext.DisposeAsync();
            _testContext = null;
        }
    }
}
