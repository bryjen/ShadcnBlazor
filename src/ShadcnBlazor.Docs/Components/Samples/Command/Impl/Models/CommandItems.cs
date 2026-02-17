using ShadcnBlazor.Docs.Components.Samples.Command.Base.Models;

namespace ShadcnBlazor.Docs.Components.Samples.Command.Impl.Models;

public sealed class PageCommandItem(string pageName) : CommandItem
{
    public string PageName => pageName;
    public override string SearchString() => pageName.Trim();
}

public sealed class ComponentCommandItem(string componentName) : CommandItem
{
    public string ComponentName => ComponentName;
    public override string SearchString() => componentName.Trim();
}

public sealed class SampleCommandItem(string sampleName) : CommandItem
{
    public string SampleName => SampleName;
    public override string SearchString() => sampleName.Trim();
}
