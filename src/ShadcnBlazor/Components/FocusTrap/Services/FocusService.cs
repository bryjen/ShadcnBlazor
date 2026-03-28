using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.FocusTrap.Models;

namespace ShadcnBlazor.Components.FocusTrap.Services;

/// <summary>
/// Handle to an active focus trap.
/// </summary>
public interface IFocusTrapHandle : IAsyncDisposable
{
    /// <summary>Activates the focus trap.</summary>
    ValueTask ActivateAsync();
    /// <summary>Deactivates the focus trap.</summary>
    ValueTask DeactivateAsync();
    /// <summary>Pauses the focus trap.</summary>
    ValueTask PauseAsync();
    /// <summary>Unpauses the focus trap.</summary>
    ValueTask UnpauseAsync();
}

/// <summary>
/// Service for creating and managing focus traps.
/// </summary>
public class FocusService
{
    private readonly FocusJsInterop _interop;

    /// <summary>
    /// Initializes a new instance of the <see cref="FocusService"/> class.
    /// </summary>
    /// <param name="interop">The focus JS interop.</param>
    public FocusService(FocusJsInterop interop)
    {
        _interop = interop;
    }

    /// <summary>
    /// Creates a focus trap for the specified element.
    /// </summary>
    /// <param name="container">The element to trap focus within.</param>
    /// <param name="options">Optional focus trap configuration.</param>
    /// <returns>A handle to the created focus trap.</returns>
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