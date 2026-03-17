using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Docs.Components.Docs.CodeBlock;

public partial class CodeBlock : ShadcnComponentBase
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter] public IReadOnlyList<CodeFile>? CodeFiles { get; set; }
    [Parameter] public CodeBlockStyle Style { get; set; } = CodeBlockStyle.Solo;
    [Parameter] public bool ShowLineNumbers { get; set; } = true;
    [Parameter] public bool ShowCopyButton { get; set; } = true;
    [Parameter] public bool Focusable { get; set; } = true;

    private readonly string _idBase = Guid.NewGuid().ToString("N")[..8];
    private IReadOnlyList<CodeFile> _files = [];
    private int _selectedIndex;
    private string _code = "";
    private string _language = "razor";
    private int[] _lineNumbers = [];
    private bool _copied;

    private string _id => $"{_idBase}-{_selectedIndex}";
    private bool HasMultipleFiles => _files.Count > 1;
    private CodeFile? Current => _files.Count > 0 ? _files[_selectedIndex] : null;

    protected override void OnParametersSet()
    {
        _files = CodeFiles is { Count: > 0 } f ? f : [];
        _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _files.Count - 1));
        SyncFromCurrent();
    }

    private void SyncFromCurrent()
    {
        _code = Current?.Contents ?? "";
        _language = Current?.Language ?? "razor";
        var lines = _code.Split('\n');
        var n = lines.Length;
        if (n > 1 && string.IsNullOrEmpty(lines[^1])) n--;
        _lineNumbers = Enumerable.Range(1, Math.Max(1, n)).ToArray();
    }

    private void SelectTab(int i)
    {
        if (i == _selectedIndex || i < 0 || i >= _files.Count) return;
        _selectedIndex = i;
        SyncFromCurrent();
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!string.IsNullOrWhiteSpace(_code))
            await JsRuntime.InvokeVoidAsync("shadcnDocsCodeblock.highlightElement", _id);
    }

    private string WrapperClass => MergeCss(
        "docs-codeblock-wrapper bg-card relative w-full max-w-full min-w-0 overflow-x-auto",
        Style == CodeBlockStyle.Embedded ? "rounded-b-lg" : "rounded-lg");

    private string CodeBlockClass => Style == CodeBlockStyle.Embedded
        ? "docs-codeblock docs-codeblock--embedded"
        : "docs-codeblock docs-codeblock--solo";

    private string TabClass(int i) => i == _selectedIndex
        ? "px-3 py-2 text-xs font-medium transition-colors duration-250 border-b-3 -mb-px border-primary text-foreground whitespace-nowrap"
        : "px-3 py-2 text-xs font-medium transition-colors duration-250 border-b-3 -mb-px border-transparent text-muted-foreground hover:text-foreground hover:border-border whitespace-nowrap";

    private string CopyBtnClass => $"absolute top-2 right-2 h-8 w-8 p-0 text-muted-foreground hover:text-foreground {(_copied ? "text-emerald-600" : "")}";

    private async Task CopyCodeAsync()
    {
        await JsRuntime.InvokeVoidAsync("shadcnDocsCodeblock.copyToClipboard", _code);
        _copied = true;
        StateHasChanged();
        await Task.Delay(2000);
        _copied = false;
        await InvokeAsync(StateHasChanged);
    }
}
