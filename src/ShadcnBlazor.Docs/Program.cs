using ShadcnBlazor.Components.Popover;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShadcnBlazor.Docs;
using TailwindMerge;
using TailwindMerge.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// builder.Services.AddSingleton<TwMerge>();
builder.Services.AddTailwindMerge();
builder.Services.AddScoped<IPopoverRegistry, PopoverRegistry>();
builder.Services.AddScoped<IPopoverService, PopoverService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
