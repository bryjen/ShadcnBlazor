namespace ShadcnBlazor.Docs.Components.Samples.Command.Base.Models;

public abstract class CommandItem
{
    public virtual string SearchString() => this.ToString() ?? string.Empty;
}