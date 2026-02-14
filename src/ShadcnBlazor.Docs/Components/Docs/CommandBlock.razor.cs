using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Shared;

namespace ShadcnBlazor.Docs.Components.Docs;

public partial class CommandBlock : ShadcnComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// Header name (tab label) to value (command) pairs. Keys become the toggle labels, values are displayed and copied.
    /// </summary>
    [Parameter]
    public IReadOnlyDictionary<string, string> Options { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Key of the option to select by default. If null, uses the first option.
    /// </summary>
    [Parameter]
    public string? DefaultOption { get; set; }

    private string? SelectedKey { get; set; }
    private bool _copied;

    protected override void OnInitialized()
    {
        UpdateSelectedOption();
    }

    protected override void OnParametersSet()
    {
        UpdateSelectedOption();
    }

    private void UpdateSelectedOption()
    {
        if (Options.Count == 0)
        {
            SelectedKey = null;
            return;
        }

        if (DefaultOption != null && Options.ContainsKey(DefaultOption))
        {
            SelectedKey = DefaultOption;
            return;
        }

        if (SelectedKey == null || !Options.ContainsKey(SelectedKey))
        {
            SelectedKey = Options.Keys.First();
        }
    }

    private void SelectOption(string key)
    {
        SelectedKey = key;
        StateHasChanged();
    }

    private async Task CopyCommandAsync()
    {
        if (SelectedKey == null || !Options.TryGetValue(SelectedKey, out var value)) return;
        await JsRuntime.InvokeVoidAsync("shadcnDocsCodeblock.copyToClipboard", value);
        _copied = true;
        StateHasChanged();
        await Task.Delay(2000);
        _copied = false;
        await InvokeAsync(StateHasChanged);
    }
}
