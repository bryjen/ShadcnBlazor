using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared.Services;

namespace ShadcnBlazor.Components.ContextMenu;

/// <summary>
/// Root container for a context menu triggered by right-click on the ContextMenuTrigger area.
/// </summary>
public partial class ContextMenu : ComponentBase
{
    [Inject]
    private ScrollLockService ScrollLock { get; set; } = default!;

    private readonly ContextMenuContext _context = new();

    /// <summary>The trigger and content of the context menu.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        _context.SetOpen = async (x, y) =>
        {
            _context.OpenAt(x, y);
            await ScrollLock.LockAsync();
            await InvokeAsync(StateHasChanged);
        };
        _context.Close = async () =>
        {
            _context.CloseMenu();
            await ScrollLock.UnlockAsync();
            await InvokeAsync(StateHasChanged);
        };
    }

    private async Task Close()
    {
        _context.CloseMenu();
        await ScrollLock.UnlockAsync();
        StateHasChanged();
    }
}
