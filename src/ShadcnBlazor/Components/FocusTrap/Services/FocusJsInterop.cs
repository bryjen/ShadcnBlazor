using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.FocusTrap.Models;

namespace ShadcnBlazor.Components.FocusTrap.Services;

/// <summary>
/// JavaScript interop for managing focus traps.
/// </summary>
public class FocusJsInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;

    /// <summary>
    /// Initializes a new instance of the <see cref="FocusJsInterop"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    public FocusJsInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync()
    {
        return _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/_content/ShadcnBlazor/Components/FocusTrap/FocusTrap.razor.js");
    }

    /// <summary>
    /// Creates a focus trap for the specified element.
    /// </summary>
    /// <param name="container">The element to trap focus within.</param>
    /// <param name="options">Optional focus trap configuration.</param>
    /// <returns>A JavaScript object reference to the trap instance.</returns>
    public async ValueTask<IJSObjectReference> CreateTrapAsync(ElementReference container, FocusOptions? options = null)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<IJSObjectReference>("createTrap", container, options ?? new FocusOptions());
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
        }
    }
}