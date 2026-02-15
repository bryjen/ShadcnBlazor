using ShadcnBlazor.Shared.Models.Options;

namespace ShadcnBlazor.Shared.Services.Interop;

using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

internal class KeyInterceptorInterop(IJSRuntime jsRuntime)
{
    public ValueTask Connect<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(DotNetObjectReference<T> dotNetObjectReference, string elementId, KeyInterceptorOptions options) where T : class
    {
        return jsRuntime.InvokeVoidAsync("mudKeyInterceptor.connect", dotNetObjectReference, elementId, options);
    }

    public ValueTask Disconnect(string elementId)
    {
        return jsRuntime.InvokeVoidAsync("mudKeyInterceptor.disconnect", elementId);
    }

    public ValueTask UpdateKey(string elementId, KeyOptions option)
    {
        return jsRuntime.InvokeVoidAsync("mudKeyInterceptor.updatekey", elementId, option);
    }
}