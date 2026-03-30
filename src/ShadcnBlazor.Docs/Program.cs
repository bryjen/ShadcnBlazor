using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShadcnBlazor.Components.Popover.Models;
using ShadcnBlazor.Components.Shared.Configuration;
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

    services.AddScoped<ThemeFetcher>();
    services.AddScoped<ThemeService>();
    services.AddScoped<ThemeInterop>();

    // Register all ShadcnBlazor services via attributes
    services.ConfigureShadcnBlazorServices();
}
