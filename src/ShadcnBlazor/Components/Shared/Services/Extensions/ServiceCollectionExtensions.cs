using Microsoft.Extensions.DependencyInjection;
using ShadcnBlazor.Components.Dialog.Services;
using ShadcnBlazor.Components.Shared.Services.Interop;

namespace ShadcnBlazor.Components.Shared.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShadcnBlazor(this IServiceCollection services)
    {
        services.AddScoped<KeyInterceptorInterop>();
        services.AddScoped<IScrollLockService, ScrollLockService>();
        services.AddScoped<IDialogService, DialogService>();

        return services;
    }
}
