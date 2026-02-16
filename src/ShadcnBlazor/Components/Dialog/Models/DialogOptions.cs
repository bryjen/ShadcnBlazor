namespace ShadcnBlazor.Components.Dialog.Models;

/// <summary>
/// Options for configuring dialog behavior and appearance.
/// </summary>
public class DialogOptions
{
    /// <summary>
    /// Whether clicking the backdrop closes the dialog.
    /// </summary>
    public bool DisableBackdropClick { get; set; }

    /// <summary>
    /// Whether pressing Escape closes the dialog.
    /// </summary>
    public bool CloseOnEscapeKey { get; set; } = true;

    /// <summary>
    /// Whether to hide the backdrop (transparent overlay).
    /// </summary>
    public bool NoBackdrop { get; set; }

    /// <summary>
    /// Whether to show a close button in the header.
    /// </summary>
    public bool CloseButton { get; set; }

    /// <summary>
    /// Whether the dialog is full screen.
    /// </summary>
    public bool FullScreen { get; set; }

    /// <summary>
    /// Whether the dialog uses full width.
    /// </summary>
    public bool FullWidth { get; set; }

    /// <summary>
    /// Whether to hide the header (title, description, close button).
    /// </summary>
    public bool NoHeader { get; set; }

    /// <summary>
    /// Maximum width of the dialog.
    /// </summary>
    public DialogMaxWidth MaxWidth { get; set; } = DialogMaxWidth.Small;

    /// <summary>
    /// Position of the dialog on screen.
    /// </summary>
    public DialogPosition Position { get; set; } = DialogPosition.Center;

    /// <summary>
    /// Optional description text shown below the title.
    /// </summary>
    public string? Description { get; set; }
}
