using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared.Services;
using ShadcnBlazor.Components.Popover.Services;
// using ShadcnBlazor.Components.Sheet.Services;
using ShadcnBlazor.Components.Shared.Models.Options;
using ShadcnBlazor.Components.Popover;
using ShadcnBlazor.Components.Popover.Models;
using ShadcnBlazor.Components.Shared.Services.Interop;
using TailwindMerge.Extensions;

namespace ShadcnBlazor.Tests.Shared.Extensions;

public static class ServiceExtensions
{
    public static void AddShadcnTestServices(this IServiceCollection services)
    {
        services.AddTailwindMerge();
        
        // Register all core services from the library
        // This ensures both UnitTests (via loose JSInterop) and Viewer (via real browser) work
        
        // Shared/Common
        services.AddScoped<ScrollLockInterop>();
        services.AddScoped<ScrollLockService>();
        services.AddScoped<KeyInterceptorInterop>();
        services.AddScoped<IKeyInterceptorService, KeyInterceptorService>();
        services.AddScoped<FocusScopeInterop>();
        services.AddScoped<IFocusScopeService, FocusScopeService>();
        
        // Popover
        services.AddScoped<PopoverInterop>();
        services.AddScoped<IPopoverRegistry, PopoverRegistry>();
        services.AddScoped<IPopoverService, PopoverService>();
        services.AddOptions<PopoverOptions>();
        
        // Dialog
        services.AddScoped<DialogInterop>();
        services.AddScoped<IDialogJsService, DialogJsService>();

        // Sheet
        // services.AddScoped<SheetInterop>();
        // services.AddScoped<ISheetService, SheetService>();
        // services.AddScoped<ISheetJsService, SheetJsService>();
    }
}
