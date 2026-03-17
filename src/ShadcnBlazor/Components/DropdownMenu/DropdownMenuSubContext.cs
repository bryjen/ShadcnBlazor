using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Components.DropdownMenu;

/// <summary>
/// Context for DropdownMenuSub and its children.
/// </summary>
public class DropdownMenuSubContext
{
    private RenderFragment? _triggerFragment;
    private RenderFragment? _contentFragment;

    /// <summary>
    /// Whether the sub-menu is open.
    /// </summary>
    public bool Open { get; set; }

    /// <summary>
    /// Sets the open state.
    /// </summary>
    public Action<bool>? SetOpen { get; set; }

    /// <summary>
    /// Cancels the close timer. Called when mouse enters this sub's content or a nested sub's content.
    /// </summary>
    public Action? CancelClose { get; set; }

    /// <summary>
    /// Parent sub context when nested. Used to cancel parent's close when entering this sub's content.
    /// </summary>
    public DropdownMenuSubContext? ParentContext { get; set; }

    /// <summary>
    /// Root dropdown menu context. Used by items in sub-menus to close the entire dropdown.
    /// </summary>
    public DropdownMenuContext? RootMenuContext { get; set; }

    /// <summary>
    /// Registers the sub-trigger content.
    /// </summary>
    public void RegisterTrigger(RenderFragment? fragment) => _triggerFragment = fragment;

    /// <summary>
    /// Registers the sub-content panel.
    /// </summary>
    public void RegisterContent(RenderFragment? fragment) => _contentFragment = fragment;

    /// <summary>
    /// Gets the registered trigger fragment.
    /// </summary>
    public RenderFragment? GetTriggerFragment() => _triggerFragment;

    /// <summary>
    /// Gets the registered content fragment.
    /// </summary>
    public RenderFragment? GetContentFragment() => _contentFragment;
}
