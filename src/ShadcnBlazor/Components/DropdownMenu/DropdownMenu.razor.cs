using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Popover;
using ShadcnBlazor.Components.Popover.Models;

namespace ShadcnBlazor.Components.DropdownMenu;

/// <summary>
/// Root container for a dropdown menu; requires PopoverProvider in layout.
/// </summary>
public partial class DropdownMenu : ComponentBase
{
    private readonly DropdownMenuContext _context = new();
    private readonly string _triggerId = $"dropdown-trigger-{Guid.NewGuid():N}";
    private bool _open;

    /// <summary>
    /// The trigger and content of the dropdown menu.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        _context.TriggerId = _triggerId;
        _context.SetOpen = v =>
        {
            _open = v;
            _context.Open = v;
            InvokeAsync(StateHasChanged);
        };
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        _context.Open = _open;
        _context.TriggerId = _triggerId;
    }
}
