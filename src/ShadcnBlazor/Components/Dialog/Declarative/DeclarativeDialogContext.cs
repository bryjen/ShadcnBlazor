using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Dialog.Declarative;

/// <summary>
/// Context object for declarative dialog components, holding state and callbacks for open/close behavior.
/// </summary>
public sealed class DeclarativeDialogContext
{
    /// <summary>Whether the dialog is currently open.</summary>
    public bool Open { get; set; }
    /// <summary>Whether the dialog is in the process of closing (animating out).</summary>
    public bool IsClosing { get; set; }
    /// <summary>Current animation state (e.g. "open", "closed").</summary>
    public string AnimationState { get; set; } = "closed";
    /// <summary>Unique identifier for the dialog instance.</summary>
    public string DialogId { get; set; } = "";
    /// <summary>Callback to close the dialog asynchronously.</summary>
    public required Func<Task> CloseAsync { get; init; }
    /// <summary>Callback to open the dialog.</summary>
    public required Action OpenDialog { get; init; }
    /// <summary>Callback to update the animation state.</summary>
    public required Action<string> SetAnimationState { get; init; }
    /// <summary>.NET object reference for JS interop callbacks.</summary>
    public DotNetObjectReference<DialogRoot>? DotNetRef { get; set; }
}
