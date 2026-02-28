using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Dialog.Services;
using ShadcnBlazor.Components.Popover.Models;
using ShadcnBlazor.Components.Popover.Services;
using ShadcnBlazor.Components.Shared.Services;
using ShadcnBlazor.Components.Shared.Services.Interop;
using ShadcnBlazor.Components.Sheet.Services;
using ShadcnBlazor.Docs;
using ShadcnBlazor.Docs.Components.Samples.AiChat.Services;
using ShadcnBlazor.Docs.Services;
using ShadcnBlazor.Docs.Services.Interop;
using ShadcnBlazor.Docs.Services.Theme;
using TailwindMerge.Extensions;

// ReSharper disable InvalidXmlDocComment

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Pre-render-safe services (called by both pre-render and runtime)
ConfigureServices(builder.Services, builder.HostEnvironment);


await builder.Build().RunAsync();

/// <summary>
/// Static method required by BlazorWasmPreRendering.Build for pre-rendering.
/// Must be named exactly "ConfigureServices" with this signature.
/// Only register services that can work during pre-rendering (no JS interop).
/// </summary>
static void ConfigureServices(IServiceCollection services, IWebAssemblyHostEnvironment hostEnv)
{
    services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(hostEnv.BaseAddress) });

    services.AddAiChat();
    services.AddTailwindMerge();
    services.AddScoped<PageTocService>();
    services.AddSingleton<ComponentRegistryService>();
    services.AddSingleton<PageRegistryService>();
    services.AddSingleton<SampleRegistryService>();

    services.AddOptions();
    services.Configure<PopoverOptions>(_ => { });
    services.AddScoped<IPopoverRegistry, PopoverRegistry>();
    services.AddScoped<IPopoverService, PopoverService>();

    services.AddScoped<IDialogService, DialogService>();
    services.AddScoped<IDialogJsService, DialogJsService>();
    services.AddScoped<ScrollLockService>();

    services.AddScoped<IKeyInterceptorService, KeyInterceptorService>();
    services.AddScoped<IFocusScopeService, FocusScopeService>();

    services.AddScoped<ISheetJsService, SheetJsService>();
    services.AddScoped<ISheetService, SheetService>();

    services.AddScoped<ThemeFetcher>();
    services.AddScoped<ThemeService>();

    services.AddScoped<ThemeInterop>();

    // JavaScript interop services (runtime only, skipped during pre-rendering)
    services.AddScoped(sp => new PopoverInterop(
        sp.GetRequiredService<IJSRuntime>(),
        ["/ShadcnBlazor/_content/ShadcnBlazor/js/popovers.js"]
    ));

    services.AddScoped(sp => new FocusScopeInterop(
        sp.GetRequiredService<IJSRuntime>(),
        ["/ShadcnBlazor/_content/ShadcnBlazor/js/focus-scope.js"]
    ));

    services.AddScoped(sp => new KeyInterceptorInterop(
        sp.GetRequiredService<IJSRuntime>(),
        ["/ShadcnBlazor/_content/ShadcnBlazor/js/key-interceptor.js"]
    ));

    services.AddScoped(sp => new ScrollLockInterop(
        sp.GetRequiredService<IJSRuntime>(),
        ["/ShadcnBlazor/_content/ShadcnBlazor/js/scroll-lock.js"]
    ));

    services.AddScoped(sp => new DialogInterop(
        sp.GetRequiredService<IJSRuntime>(),
        ["/ShadcnBlazor/_content/ShadcnBlazor/js/dialog.js"]
    ));

    services.AddScoped(sp => new SheetInterop(
        sp.GetRequiredService<IJSRuntime>(),
        ["/ShadcnBlazor/_content/ShadcnBlazor/js/sheet.js"]
    ));
}
