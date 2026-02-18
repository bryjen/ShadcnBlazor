using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShadcnBlazor.Components.Dialog.Services;
using ShadcnBlazor.Components.Sheet.Services;
using ShadcnBlazor.Components.Popover.Services;
using ShadcnBlazor.Components.Shared.Services;
using ShadcnBlazor.Docs.Services;
using ShadcnBlazor.Docs;
using ShadcnBlazor.Docs.Components.Samples.AiChat.Services;
using TailwindMerge.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddAiChat();

// builder.Services.AddSingleton<TwMerge>();
builder.Services.AddTailwindMerge();
builder.Services.AddScoped<PageTocService>();
builder.Services.AddSingleton<ComponentRegistryService>();
builder.Services.AddSingleton<PageRegistryService>();
builder.Services.AddSingleton<SampleRegistryService>();

builder.Services.AddScoped<IPopoverRegistry, PopoverRegistry>();
builder.Services.AddScoped<IPopoverService, PopoverService>();

builder.Services.AddScoped<IScrollLockService, ScrollLockService>();
builder.Services.AddScoped<IDialogService, DialogService>();
builder.Services.AddScoped<ISheetService, SheetService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
