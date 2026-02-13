using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Shared;

namespace ShadcnBlazor.Docs.Components.Docs;

public partial class CodeBlock : ShadcnComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter]
    public string Code { get; set; } = "";

    [Parameter]
    public string Language { get; set; } = "razor";

    /// <summary>
    /// Solo: standalone code block with no border, full rounded corners.
    /// Embedded: designed to sit below content (e.g. in ComponentPreview), top border when in context, bottom rounded only.
    /// </summary>
    [Parameter]
    public CodeBlockStyle Style { get; set; } = CodeBlockStyle.Solo;

    [Parameter]
    public bool ShowLineNumbers { get; set; } = true;

    [Parameter]
    public bool ShowCopyButton { get; set; } = true;

    private string _id = Guid.NewGuid().ToString("N")[..8];
    private int[] _lineNumbers = Array.Empty<int>();
    private bool _copied;

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
        if (!string.IsNullOrWhiteSpace(Code))
        {
            await JsRuntime.InvokeVoidAsync("shadcnDocsCodeblock.highlightElement", _id);
        }
    }

    private string GetWrapperClass()
    {
        var baseClasses = "bg-card relative w-full max-w-full min-w-0 overflow-x-auto ";
        baseClasses += Style switch
        {
            CodeBlockStyle.Solo => "rounded-lg",
            CodeBlockStyle.Embedded => "rounded-b-lg",
            _ => "rounded-lg"
        };
        return MergeCss(baseClasses);
    }

    private string GetCodeBlockClass()
    {
        var variantClass = Style switch
        {
            CodeBlockStyle.Solo => "docs-codeblock docs-codeblock--solo",
            CodeBlockStyle.Embedded => "docs-codeblock docs-codeblock--embedded",
            _ => "docs-codeblock docs-codeblock--solo"
        };
        return variantClass;
    }

    private string GetCopyButtonClass() =>
        $"absolute top-2 right-2 h-8 w-8 p-0 text-muted-foreground hover:text-foreground transition-colors duration-200 {(_copied ? "text-emerald-600" : "")}";

    private async Task CopyCodeAsync()
    {
        await JsRuntime.InvokeVoidAsync("shadcnDocsCodeblock.copyToClipboard", Code);
        _copied = true;
        StateHasChanged();
        await Task.Delay(2000);
        _copied = false;
        await InvokeAsync(StateHasChanged);
    }
}
