using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.Select;

/// <summary>
/// Dropdown select component for choosing a single value from a list of options.
/// </summary>
public partial class Select<T>
{
    [Parameter]
    public string? Label { get; set; }
    [Parameter]
    public T? Value { get; set; }
    [Parameter]
    public EventCallback<T?> ValueChanged { get; set; }
    [Parameter, EditorRequired]
    public required IEnumerable<SelectOption<T>> Items { get; set; }
    [Parameter]
    public string Placeholder { get; set; } = "Select...";
    [Parameter]
    public Size Size { get; set; } = Size.Md;

    /// <summary>
    /// When true, the popover expands to fit the width of its option content instead of matching the trigger width.
    /// </summary>
    [Parameter]
    public bool PopoverFitContent { get; set; }
    [Parameter]
    public bool LockScroll { get; set; } = true;
    [Parameter]
    public bool Disabled { get; set; }
    [Parameter]
    public string TriggerClass { get; set; } = string.Empty;

    /// <summary>
    /// Maximum number of visible options before the list becomes scrollable. When null or less than 1, no item-count cap is applied.
    /// </summary>
    [Parameter]
    public int? MaxVisibleItems { get; set; }

    [Parameter]
    public RenderFragment<SelectOption<T>>? RenderItem { get; set; }

    private readonly string _triggerId = $"select-trigger-{Guid.NewGuid():N}";
    private readonly string _listboxId = $"select-listbox-{Guid.NewGuid():N}";
    private bool _open;
    private int? _activeIndex;
    private string? _pendingScrollOptionId;
    private IJSObjectReference? _module;
    private bool _maxVisibleItemsListenerRegistered;
    private int _lastMeasuredOptionCount = -1;
    private int? _lastMeasuredMaxVisibleItems;

    private string GetContentClass()
    {
        return PopoverFitContent
            ? "w-max"
            : "w-[var(--popover-width)]";
    }

    private string GetTriggerClass()
    {
        var baseClasses = string.Join(" ", [
            "flex w-full items-center justify-between gap-2 rounded-md border border-input",
            "bg-input/30 shadow-xs transition-colors hover:bg-input/50",
            "focus-visible:outline-none disabled:cursor-not-allowed disabled:opacity-50",
            "[&>svg]:size-4 [&>svg]:shrink-0",
        ]);
        var sizeClasses = Size switch
        {
            Size.Sm => "h-6 px-1.75 py-0.75 text-[0.6rem]",
            Size.Md => "h-7 px-2.5 py-1 text-sm",
            Size.Lg => "h-8 px-2.75 py-1.25 text-base md:text-sm",
            _ => "h-7 px-2.5 py-1 text-sm",
        };
        return MergeCss(baseClasses, sizeClasses, TriggerClass);
    }

    private string GetOptionClass(bool isSelected, bool isActive, bool isDisabled)
    {
        var selectedClasses = isSelected
            ? "bg-primary text-primary-foreground"
            : string.Empty;
        var activeClasses = isActive ? "duration-0 bg-primary/30" : string.Empty;
        var disabledClasses = isDisabled ? "cursor-not-allowed opacity-50" : string.Empty;
        var baseClasses = string.Join(" ", [
            "flex w-full items-center gap-2 rounded-md px-2 py-1.5 text-sm",
            "outline-none focus-visible:outline-none transition-colors",
        ]);
        var interactiveClasses = isDisabled ? string.Empty : "cursor-pointer";
        return MergeCss(activeClasses, selectedClasses, disabledClasses, interactiveClasses, baseClasses);
    }

    private string GetAriaLabel()
    {
        if (!string.IsNullOrWhiteSpace(Label))
            return Label;

        return Placeholder;
    }

    private string GetDisplayText()
    {
        if (Value is null)
            return Placeholder;

        var option = GetOptions().FirstOrDefault(o => EqualityComparer<T>.Default.Equals(o.Value, Value));
        return option.DisplayText;
    }

    private bool IsSelected(T? value)
    {
        if (value is null && Value is null) return true;
        if (value is null || Value is null) return false;
        return EqualityComparer<T>.Default.Equals(value, Value);
    }

    private string? GetActiveDescendant()
    {
        if (!_open || _activeIndex is null)
            return null;

        return GetOptionId(_activeIndex.Value);
    }

    private string GetOptionId(int index) => $"{_listboxId}-option-{index}";

    private List<SelectOption<T>> GetOptions() => [.. Items];

    private int? GetSelectedIndex(List<SelectOption<T>> options)
    {
        for (var i = 0; i < options.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(options[i].Value, Value))
                return i;
        }

        return null;
    }

    private static bool IsIndexEnabled(List<SelectOption<T>> options, int index)
    {
        return index >= 0 && index < options.Count && !options[index].Disabled;
    }

    private static int? GetFirstEnabledIndex(List<SelectOption<T>> options)
    {
        for (var i = 0; i < options.Count; i++)
        {
            if (!options[i].Disabled)
                return i;
        }

        return null;
    }

    private static int? GetLastEnabledIndex(List<SelectOption<T>> options)
    {
        for (var i = options.Count - 1; i >= 0; i--)
        {
            if (!options[i].Disabled)
                return i;
        }

        return null;
    }

    private static int? GetNextEnabledIndex(List<SelectOption<T>> options, int? current)
    {
        if (options.Count == 0)
            return null;

        var start = current ?? -1;
        for (var step = 1; step <= options.Count; step++)
        {
            var i = (start + step) % options.Count;
            if (!options[i].Disabled)
                return i;
        }

        return null;
    }

    private static int? GetPreviousEnabledIndex(List<SelectOption<T>> options, int? current)
    {
        if (options.Count == 0)
            return null;

        var start = current ?? 0;
        for (var step = 1; step <= options.Count; step++)
        {
            var i = (start - step + options.Count) % options.Count;
            if (!options[i].Disabled)
                return i;
        }

        return null;
    }

    private void SetActiveIndex(int? index)
    {
        _activeIndex = index;
        if (_open && index is not null)
        {
            _pendingScrollOptionId = GetOptionId(index.Value);
        }
    }

    private void HandleOptionMouseEnter(int index)
    {
        var options = GetOptions();
        if (!IsIndexEnabled(options, index))
            return;

        SetActiveIndex(index);
    }

    private void EnsureActiveOnOpen()
    {
        var options = GetOptions();
        if (options.Count == 0)
        {
            SetActiveIndex(null);
            return;
        }

        var selectedIndex = GetSelectedIndex(options);
        if (selectedIndex is not null && IsIndexEnabled(options, selectedIndex.Value))
        {
            SetActiveIndex(selectedIndex);
            return;
        }

        SetActiveIndex(GetFirstEnabledIndex(options));
    }

    private Task HandleOpenChanged(bool open)
    {
        _open = open;
        if (_open)
        {
            EnsureActiveOnOpen();
        }

        return Task.CompletedTask;
    }

    private Task ToggleOpen()
    {
        if (Disabled)
            return Task.CompletedTask;

        _open = !_open;
        if (_open)
        {
            EnsureActiveOnOpen();
        }

        return Task.CompletedTask;
    }

    private async Task HandleTriggerKeyDown(KeyboardEventArgs e)
    {
        if (Disabled)
            return;

        var options = GetOptions();
        if (options.Count == 0)
            return;

        if (GetFirstEnabledIndex(options) is null)
            return;

        // Single-character typeahead: jump to the next enabled option whose text starts with the typed letter.
        if (TryHandleFirstLetterTypeahead(e.Key, options))
        {
            if (!_open)
            {
                _open = true;
            }

            await InvokeAsync(StateHasChanged);
            return;
        }

        switch (e.Key)
        {
            case "ArrowDown":
            case "Down":
                if (!_open)
                {
                    _open = true;
                    EnsureActiveOnOpen();
                }
                else
                {
                    SetActiveIndex(GetNextEnabledIndex(options, _activeIndex));
                }
                break;
            case "ArrowUp":
            case "Up":
                if (!_open)
                {
                    _open = true;
                    EnsureActiveOnOpen();
                }
                else
                {
                    SetActiveIndex(GetPreviousEnabledIndex(options, _activeIndex));
                }
                break;
            case "Home":
                if (!_open)
                    _open = true;
                SetActiveIndex(GetFirstEnabledIndex(options));
                break;
            case "End":
                if (!_open)
                    _open = true;
                SetActiveIndex(GetLastEnabledIndex(options));
                break;
            case "Enter":
            case " ":
                if (!_open)
                {
                    _open = true;
                    EnsureActiveOnOpen();
                }
                else if (_activeIndex is not null)
                {
                    await SelectOptionAtIndexAsync(_activeIndex.Value);
                }
                break;
            case "Escape":
            case "Esc":
                _open = false;
                break;
            case "Tab":
                _open = false;
                break;
        }

        await InvokeAsync(StateHasChanged);
    }

    private bool TryHandleFirstLetterTypeahead(string? key, List<SelectOption<T>> options)
    {
        if (string.IsNullOrWhiteSpace(key) || key!.Length != 1)
            return false;

        var ch = key[0];
        if (char.IsControl(ch) || char.IsWhiteSpace(ch))
            return false;

        var start = (_activeIndex ?? -1) + 1;
        for (var offset = 0; offset < options.Count; offset++)
        {
            var i = (start + offset) % options.Count;
            if (options[i].Disabled)
                continue;

            var text = options[i].DisplayText;
            if (!string.IsNullOrEmpty(text) &&
                char.ToUpperInvariant(text[0]) == char.ToUpperInvariant(ch))
            {
                SetActiveIndex(i);
                return true;
            }
        }

        return false;
    }

    private async Task SelectOptionAtIndexAsync(int index)
    {
        var options = GetOptions();
        if (index < 0 || index >= options.Count)
            return;

        if (options[index].Disabled)
            return;

        Value = options[index].Value;
        SetActiveIndex(index);
        await ValueChanged.InvokeAsync(Value);
        _open = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_open && !string.IsNullOrEmpty(_pendingScrollOptionId))
        {
            await EnsureModuleAsync();
            if (_module is not null)
            {
                await _module.InvokeVoidAsync("ensureOptionVisible", _listboxId, _pendingScrollOptionId);
            }
            _pendingScrollOptionId = null;
        }

        if (_open && ShouldApplyMaxVisibleItems() && NeedsMaxVisibleItemsMeasurement())
        {
            await EnsureModuleAsync();
            if (_module is not null && MaxVisibleItems is not null)
            {
                await _module.InvokeVoidAsync("applyListboxMaxVisibleItems", _listboxId, MaxVisibleItems.Value, 96);
                _maxVisibleItemsListenerRegistered = true;
                MarkMaxVisibleItemsMeasured();
            }
        }

        if (!_open && _maxVisibleItemsListenerRegistered)
        {
            await DisposeMaxVisibleItemsListenerAsync();
            ResetMaxVisibleItemsMeasurementState();
        }
    }

    private bool ShouldApplyMaxVisibleItems() => MaxVisibleItems is > 0;

    private bool NeedsMaxVisibleItemsMeasurement()
    {
        var optionCount = GetOptions().Count;
        return !_maxVisibleItemsListenerRegistered
               || _lastMeasuredOptionCount != optionCount
               || _lastMeasuredMaxVisibleItems != MaxVisibleItems;
    }

    private void MarkMaxVisibleItemsMeasured()
    {
        _lastMeasuredOptionCount = GetOptions().Count;
        _lastMeasuredMaxVisibleItems = MaxVisibleItems;
    }

    private void ResetMaxVisibleItemsMeasurementState()
    {
        _maxVisibleItemsListenerRegistered = false;
        _lastMeasuredOptionCount = -1;
        _lastMeasuredMaxVisibleItems = null;
    }

    private async Task DisposeMaxVisibleItemsListenerAsync()
    {
        if (_module is null)
            return;

        await _module.InvokeVoidAsync("disposeListboxMaxVisibleItems", _listboxId);
    }

    private async Task EnsureModuleAsync()
    {
        if (_module is not null)
            return;

        _module = await JsRuntime.InvokeAsync<IJSObjectReference>(
            "import",
            "/ShadcnBlazor/_content/ShadcnBlazor/Components/Select/Select.razor.js");
    }

    public async ValueTask DisposeAsync()
    {
        if (_maxVisibleItemsListenerRegistered)
        {
            await DisposeMaxVisibleItemsListenerAsync();
            ResetMaxVisibleItemsMeasurementState();
        }

        if (_module is not null)
        {
            await _module.DisposeAsync();
            _module = null;
        }
    }
}
