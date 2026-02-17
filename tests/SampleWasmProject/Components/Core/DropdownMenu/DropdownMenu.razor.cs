using Microsoft.AspNetCore.Components;
using SampleWasmProject.Components.Core.Popover;
using SampleWasmProject.Components.Core.Popover.Models;

namespace SampleWasmProject.Components.Core.DropdownMenu;

/// <summary>
/// Root container for a dropdown menu; requires PopoverProvider in layout.
/// </summary>
public partial class DropdownMenu : ComponentBase
{
    private readonly DropdownMenuContext _context = new();
    private bool _open;

    /// <summary>
    /// The trigger and content of the dropdown menu.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
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
    }
}
