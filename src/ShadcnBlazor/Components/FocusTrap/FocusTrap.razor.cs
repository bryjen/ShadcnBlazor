using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.FocusTrap.Models;
using ShadcnBlazor.Components.FocusTrap.Services;
using ShadcnBlazor.Components.Shared;
using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.FocusTrap;

/// <summary>
/// Component that traps keyboard focus within its child content.
/// </summary>
public partial class FocusTrap : ShadcnComponentBase, IAsyncDisposable
{
    /// <summary>
    /// Gets or sets the content to trap focus within.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets whether the focus trap is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the options for the focus trap.
    /// </summary>
    [Parameter]
    public FocusOptions? Options { get; set; }

    [Inject]
    private FocusService FocusService { get; set; } = default!;

    private ElementReference _container;
    private IFocusTrapHandle? _trapHandle;
    private bool _isInitialized;
    private bool _wasDisabled = true;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _trapHandle = await FocusService.CreateTrapAsync(_container, Options);
            _isInitialized = true;
            _wasDisabled = Disabled;
            
            if (!Disabled)
            {
                await _trapHandle.ActivateAsync();
            }
        }
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        if (_isInitialized && _trapHandle is not null && _wasDisabled != Disabled)
        {
            _wasDisabled = Disabled;
            if (Disabled)
            {
                await _trapHandle.DeactivateAsync();
            }
            else
            {
                await _trapHandle.ActivateAsync();
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_trapHandle is not null)
        {
            try 
            {
                await _trapHandle.DeactivateAsync();
                await _trapHandle.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Ignore if the JS runtime is already gone.
            }
        }
    }
}