namespace ShadcnBlazor.Components.Shared.Models.Accessibility;

/// <summary>Accessibility context for a popover trigger.</summary>
public class PopoverTriggerContext
{
    /// <summary>Whether the popover is open.</summary>
    public bool Open { get; set; }
    /// <summary>Id of the popover element.</summary>
    public string PopoverId { get; set; } = "";
    /// <summary>Value for <c>aria-haspopup</c>.</summary>
    public string AriaHasPopup { get; set; } = "dialog"; // "menu" | "listbox" | "dialog"
}
