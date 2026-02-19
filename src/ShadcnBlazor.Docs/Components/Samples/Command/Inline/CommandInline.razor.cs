using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Options;
using ShadcnBlazor.Components.Shared.Services;
using ShadcnBlazor.Docs.Components.Samples.Command.Base.Models;
using ShadcnBlazor.Docs.Components.Samples.Command.Impl.Models;
using ShadcnBlazor.Docs.Services;

namespace ShadcnBlazor.Docs.Components.Samples.Command.Inline;

public partial class CommandInline : ShadcnComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public IKeyInterceptorService KeyInterceptor { get; set; } = null!;

    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    private PageRegistryService PageRegistry { get; set; } = null!;

    [Inject]
    private ComponentRegistryService ComponentRegistry { get; set; } = null!;

    [Inject]
    private SampleRegistryService SampleRegistry { get; set; } = null!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private IReadOnlyList<CommandItem>? _commandItemsCache;

    private IReadOnlyList<CommandItem> CommandItems =>
        _commandItemsCache ??= BuildCommandItems();

    private string _searchQuery = string.Empty;

    private bool _openRequested;

    // current item selection
    private int? _selectedItem;  // hash of item
    private int? _pendingScrollItem;
    
    // side-effect-ful
    private IReadOnlyList<CommandItem> GetFilteredCommandItems()
    {
        var filtered = DefaultFilter(CommandItems, _searchQuery);

        if (_selectedItem is null ||
            !filtered.Select(t => t?.GetHashCode()).Where(t => t is not null).Contains(_selectedItem))
        {
            _selectedItem = filtered.FirstOrDefault()?.GetHashCode();
            _pendingScrollItem = _selectedItem;
        }

        return filtered;
    }

    private IReadOnlyList<CommandItem> BuildCommandItems()
    {
        var items = PageRegistry.PagesList.Select(page => new PageCommandItem(page.Name)).Cast<CommandItem>().ToList();
        items.AddRange(ComponentRegistry.Components.Select(component => new ComponentCommandItem(component.Name)).Cast<CommandItem>());
        items.AddRange(SampleRegistry.SamplesList.Select(sample => new SampleCommandItem(sample.Name)).Cast<CommandItem>());
        return items;
    }

    private IReadOnlyList<CommandItem> DefaultFilter(IReadOnlyList<CommandItem> commandItems, string searchQuery)
    {
        var query = (searchQuery ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(query))
            return commandItems;

        var terms = query.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => t.Length > 0)
            .ToArray();
        if (terms.Length == 0)
            return commandItems;

        return commandItems
            .Where(item =>
            {
                var text = item.SearchString();
                if (string.IsNullOrEmpty(text))
                    return false;
                return terms.All(term =>
                    text.Contains(term, StringComparison.OrdinalIgnoreCase));
            })
            .ToList();
    }

    private void HandleHover(int elementId)
    {
        _selectedItem = elementId;
        _pendingScrollItem = _selectedItem;
        InvokeAsync(StateHasChanged);
    }
    
    private void NavigateToPage(CommandItem commandItem)
    {
        var href = commandItem switch
        {
            PageCommandItem pageCommand => pageCommand.PageName,
            ComponentCommandItem componentCommand => $"components/{componentCommand.ComponentName}",
            SampleCommandItem sampleCommand => $"samples/{sampleCommand.SampleName}",
            _ => string.Empty
        };

        NavigationManager.NavigateTo(href);
    }

    private void OnTriggerClick()
    {
        _openRequested = true;
        _selectedItem = null;
        _pendingScrollItem = null;
        _ = InvokeAsync(StateHasChanged);
    }

    private void OnTriggerKeyDown(KeyboardEventArgs e)
    {
        if (e.Key is "Enter" or " " )
        {
            _openRequested = true;
            _selectedItem = null;
            _pendingScrollItem = null;
            _ = InvokeAsync(StateHasChanged);
        }
    }

    private async ValueTask EnsureScrollModuleAsync()
    {
        if (_scrollModule is not null)
            return;

        var modulePath = "./Components/Samples/Command/Inline/CommandInline.razor.js";
        _scrollModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", modulePath);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_openRequested)
        {
            _dotNetRef ??= DotNetObjectReference.Create(this);
            var options = new KeyInterceptorOptions(
                new KeyOptions("Tab", subscribeDown: true, preventDown: "key+none", stopDown: "key+none"),
                new KeyOptions("ArrowUp", subscribeDown: true, preventDown: "key+none", stopDown: "key+none"),
                new KeyOptions("ArrowDown", subscribeDown: true, preventDown: "key+none", stopDown: "key+none"),
                new KeyOptions("Enter", subscribeDown: true, preventDown: "key+none", stopDown: "key+none")
            );
            try
            {
                await KeyInterceptor.ConnectAsync(_elementId, _dotNetRef, options);
                await EnsureScrollModuleAsync();
            }
            catch
            {
                // dialog content not mounted yet
            }
            _openRequested = false;
        }

        if (_pendingScrollItem is not null)
        {
            await EnsureScrollModuleAsync();
            if (_scrollModule is not null)
            {
                await _scrollModule.InvokeVoidAsync("ensureSelectedVisible", _contentId, _pendingScrollItem.Value.ToString());
                _pendingScrollItem = null;
            }
        }
    }
    
    public void Dispose()
    {
        _ = KeyInterceptor.DisconnectAsync(_elementId);
        _dotNetRef?.Dispose();
        
        if (_scrollModule is not null)
        {
            _ = _scrollModule.DisposeAsync();
            _scrollModule = null;
        }
    }
}
