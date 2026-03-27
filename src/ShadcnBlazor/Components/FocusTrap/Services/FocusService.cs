using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.FocusTrap.Models;

namespace ShadcnBlazor.Components.FocusTrap.Services;

public interface IFocusTrapHandle : IAsyncDisposable
{
    ValueTask ActivateAsync();
    ValueTask DeactivateAsync();
    ValueTask PauseAsync();
    ValueTask UnpauseAsync();
}

public class FocusService
{
    private readonly FocusJsInterop _interop;

    public FocusService(FocusJsInterop interop)
    {
        _interop = interop;
    }

    public async ValueTask<IFocusTrapHandle> CreateTrapAsync(ElementReference container, FocusOptions? options = null)
    {
        var trapRef = await _interop.CreateTrapAsync(container, options);
        return new FocusTrapHandle(trapRef);
    }
}

internal class FocusTrapHandle : IFocusTrapHandle
{
    private IJSObjectReference? _trapRef;

    public FocusTrapHandle(IJSObjectReference trapRef)
    {
        _trapRef = trapRef;
    }

    public async ValueTask ActivateAsync()
    {
        if (_trapRef is not null) await _trapRef.InvokeVoidAsync("activate");
    }

    public async ValueTask DeactivateAsync()
    {
        if (_trapRef is not null) await _trapRef.InvokeVoidAsync("deactivate");
    }

    public async ValueTask PauseAsync()
    {
        if (_trapRef is not null) await _trapRef.InvokeVoidAsync("pause");
    }

    public async ValueTask UnpauseAsync()
    {
        if (_trapRef is not null) await _trapRef.InvokeVoidAsync("unpause");
    }

    public async ValueTask DisposeAsync()
    {
        if (_trapRef is not null)
        {
            await _trapRef.DisposeAsync();
            _trapRef = null;
        }
    }
}