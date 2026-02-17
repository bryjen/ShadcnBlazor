using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SampleWasmProject;
using TailwindMerge.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddTailwindMerge();
        builder.Services.AddScoped<SampleWasmProject.Components.Core.Popover.Services.IPopoverService, SampleWasmProject.Components.Core.Popover.Services.PopoverService>()
        builder.Services.AddScoped<SampleWasmProject.Components.Core.Popover.Services.IPopoverRegistry, SampleWasmProject.Components.Core.Popover.Services.PopoverRegistry>()
        builder.Services.AddScoped<SampleWasmProject.Components.Core.Dialog.Services.IDialogService, SampleWasmProject.Components.Core.Dialog.Services.DialogService>()
        builder.Services.AddScoped<SampleWasmProject.Components.Core.Shared.Services.IScrollLockService, SampleWasmProject.Components.Core.Shared.Services.ScrollLockService>()
    await builder.Build().RunAsync();