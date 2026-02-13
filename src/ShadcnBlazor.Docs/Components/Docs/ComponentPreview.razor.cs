using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ShadcnBlazor.Docs.Components.Docs;

public partial class ComponentPreview
{
    [Inject] 
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Parameter] 
    public RenderFragment? ChildContent { get; set; }
    [Parameter] 
    public string Code { get; set; } = "";
    [Parameter] 
    public string Language { get; set; } = "razor";
    
    private string _id = Guid.NewGuid().ToString("N")[..8];
    private int[] _lineNumbers = Array.Empty<int>();

    protected override void OnParametersSet()
    {
        var lines = Code.Split('\n');
        var count = lines.Length;
        if (count > 1 && string.IsNullOrEmpty(lines[^1]))
            count--;
        _lineNumbers = Enumerable.Range(1, Math.Max(1, count)).ToArray();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JsRuntime.InvokeVoidAsync("shadcnDocsCodeblock.highlightElement", _id);
    }

    private async Task CopyToClipboard()
    {
        await JsRuntime.InvokeVoidAsync("shadcnDocsCodeblock.copyToClipboard", Code);
    }
}