namespace ShadcnBlazor.Docs.Components.Samples.Command.Models;

public abstract class CommandItem
{
    public virtual string SearchString() => this.ToString() ?? string.Empty;
}