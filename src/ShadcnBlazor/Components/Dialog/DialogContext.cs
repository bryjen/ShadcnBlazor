using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Dialog;

/// <summary>
/// Context object for dialog components, holding state and callbacks for open/close behavior.
/// </summary>
public sealed class DialogContext
{
    /// <summary>Whether the dialog is currently open.</summary>
    public bool Open { get; set; }
    /// <summary>Whether the dialog is in the process of closing (animating out).</summary>
    public bool IsClosing { get; set; }
    /// <summary>Current animation state (e.g. "open", "closed").</summary>
    public string AnimationState { get; set; } = "closed";
    /// <summary>Unique identifier for the dialog instance.</summary>
    public string DialogId { get; set; } = "";
    /// <summary>Whether clicking the backdrop closes the dialog.</summary>
    public bool CloseOnBackdropClick { get; set; } = true;
    /// <summary>Callback to close the dialog asynchronously.</summary>
    public required Func<Task> CloseAsync { get; init; }
    /// <summary>Callback to open the dialog.</summary>
    public required Action OpenDialog { get; init; }
    /// <summary>Callback to update the animation state.</summary>
    public required Action<string> SetAnimationState { get; init; }
    /// <summary>Callback invoked when dialog closes via overlay click.</summary>
    public Func<Task>? OnClosedByOverlay { get; set; }
    /// <summary>Callback invoked when dialog closes via Escape key.</summary>
    public Func<Task>? OnClosedByEscape { get; set; }
    /// <summary>.NET object reference for JS interop callbacks.</summary>
    public DotNetObjectReference<DialogRoot>? DotNetRef { get; set; }
}
