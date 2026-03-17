using ShadcnBlazor.Docs.Services;

namespace ShadcnBlazor.Docs.Components.Samples.Command.Models;

public sealed class PageCommandItem(PageRegistryEntry pageRegistryEntry) : CommandItem
{
    public string PageName => pageRegistryEntry.Name;
    public override string SearchString() => pageRegistryEntry.Name.Trim();
}

public sealed class ComponentCommandItem(string componentName) : CommandItem
{
    public string ComponentName => componentName;
    public override string SearchString() => componentName.Trim();
}

public sealed class SampleCommandItem(string sampleName) : CommandItem
{
    public string SampleName => sampleName;
    public override string SearchString() => sampleName.Trim();
}
