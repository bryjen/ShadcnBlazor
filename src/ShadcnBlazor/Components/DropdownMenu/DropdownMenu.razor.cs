using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Popover;
using ShadcnBlazor.Components.Popover.Models;
using ShadcnBlazor.Shared.Attributes;

namespace ShadcnBlazor.Components.DropdownMenu;

/// <summary>
/// Root container for a dropdown menu; requires PopoverProvider in layout.
/// </summary>
[ComponentMetadata(Name = nameof(DropdownMenu), Description = "Dropdown menu with trigger and content; requires PopoverProvider in layout.", Dependencies = ["Popover"])]
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
