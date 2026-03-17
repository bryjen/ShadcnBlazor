using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Components.DropdownMenu;

/// <summary>
/// Shared context for DropdownMenu and its child components.
/// </summary>
public class DropdownMenuContext
{
    private RenderFragment? _triggerFragment;
    private RenderFragment? _contentFragment;

    /// <summary>
    /// Whether the dropdown menu is open.
    /// </summary>
    public bool Open { get; set; }

    /// <summary>
    /// Trigger element id used for focus restoration.
    /// </summary>
    public string TriggerId { get; set; } = string.Empty;

    /// <summary>
    /// Sets the open state.
    /// </summary>
    public Action<bool>? SetOpen { get; set; }

    /// <summary>
    /// Registers the trigger content.
    /// </summary>
    public void RegisterTrigger(RenderFragment? fragment)
    {
        _triggerFragment = fragment;
    }

    /// <summary>
    /// Registers the content panel.
    /// </summary>
    public void RegisterContent(RenderFragment? fragment)
    {
        _contentFragment = fragment;
    }

    /// <summary>
    /// Gets the registered trigger fragment.
    /// </summary>
    public RenderFragment? GetTriggerFragment() => _triggerFragment;

    /// <summary>
    /// Gets the registered content fragment.
    /// </summary>
    public RenderFragment? GetContentFragment() => _contentFragment;
}
