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
using ShadcnBlazor.Docs.Services.Theme;
using TailwindMerge.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddAiChat();
builder.Services.AddTailwindMerge();
builder.Services.AddScoped<PageTocService>();
builder.Services.AddSingleton<ComponentRegistryService>();
builder.Services.AddSingleton<PageRegistryService>();
builder.Services.AddSingleton<SampleRegistryService>();

builder.Services.AddOptions();
builder.Services.Configure<PopoverOptions>(_ => { });
builder.Services.AddScoped<IPopoverRegistry, PopoverRegistry>();
builder.Services.AddScoped<IPopoverService, PopoverService>();

builder.Services.AddScoped<IDialogService, DialogService>();
builder.Services.AddScoped<IDialogJsService, DialogJsService>();
builder.Services.AddScoped<ScrollLockService>();

builder.Services.AddScoped(sp => new PopoverInterop(
    sp.GetRequiredService<IJSRuntime>(),
    PopoverInterop.DefaultModulePaths));

builder.Services.AddScoped(sp => new FocusScopeInterop(
    sp.GetRequiredService<IJSRuntime>(),
    FocusScopeInterop.DefaultModulePaths));
builder.Services.AddScoped<IFocusScopeService, FocusScopeService>();

builder.Services.AddScoped(sp => new KeyInterceptorInterop(
    sp.GetRequiredService<IJSRuntime>(),
    KeyInterceptorInterop.DefaultModulePaths));
builder.Services.AddScoped<IKeyInterceptorService, KeyInterceptorService>();

builder.Services.AddScoped(sp => new ScrollLockInterop(
    sp.GetRequiredService<IJSRuntime>(),
    ScrollLockInterop.DefaultModulePaths));

builder.Services.AddScoped(sp => new DialogInterop(
    sp.GetRequiredService<IJSRuntime>(),
    DialogInterop.DefaultModulePaths));

builder.Services.AddScoped(sp => new SheetInterop(
    sp.GetRequiredService<IJSRuntime>(),
    SheetInterop.DefaultModulePaths));
builder.Services.AddScoped<ISheetJsService, SheetJsService>();
builder.Services.AddScoped<ISheetService, SheetService>();

builder.Services.AddScoped<ThemeFetcher>();
builder.Services.AddScoped<ThemeService>();

await builder.Build().RunAsync();


