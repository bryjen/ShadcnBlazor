using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Docs.Components.Docs;

public partial class CommandBlock : ShadcnComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter]
    public string Command { get; set; } = string.Empty;

    private readonly string _codeId = $"command-block-{Guid.NewGuid():N}";
    private bool _copied;
    private string? _lastCommand;
    private bool _highlighted;

    protected override void OnParametersSet()
    {
        if (!string.Equals(_lastCommand, Command, StringComparison.Ordinal))
        {
            _lastCommand = Command;
            _highlighted = false;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_highlighted)
        {
            return;
        }

        await JsRuntime.InvokeVoidAsync("shadcnDocsCodeblock.highlightElement", _codeId);
        _highlighted = true;
    }

    private async Task CopyCommandAsync()
    {
        if (string.IsNullOrWhiteSpace(Command))
        {
            return;
        }

        await JsRuntime.InvokeVoidAsync("shadcnDocsCodeblock.copyToClipboard", Command);
        _copied = true;
        StateHasChanged();
        await Task.Delay(2000);
        _copied = false;
        await InvokeAsync(StateHasChanged);
    }
}
