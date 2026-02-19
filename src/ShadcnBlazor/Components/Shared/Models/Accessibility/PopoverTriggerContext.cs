namespace ShadcnBlazor.Components.Shared.Models.Accessibility;

public class PopoverTriggerContext
{
    public bool Open { get; set; }
    public string PopoverId { get; set; } = "";
    public string AriaHasPopup { get; set; } = "dialog"; // "menu" | "listbox" | "dialog"
}