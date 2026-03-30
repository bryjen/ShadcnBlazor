using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.Slider;

/// <summary>
/// Unified slider component supporting single or multiple thumbs.
/// Uses div-based thumbs with pointer event capture, matching Radix UI behavior.
/// </summary>
public partial class Slider : ShadcnComponentBase, IAsyncDisposable
{
    [Inject] IJSRuntime JS { get; set; } = default!;

    /// <summary>
    /// Single-thumb value (convenience mode).
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Data)]
    public double Value { get; set; }

    /// <summary>
    /// Callback for single-thumb mode.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public EventCallback<double> ValueChanged { get; set; }

    /// <summary>
    /// Multi-thumb values.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Data)]
    public List<double>? Values { get; set; }

    /// <summary>
    /// Callback for multi-thumb mode.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public EventCallback<List<double>> ValuesChanged { get; set; }

    /// <summary>
    /// Minimum value of the slider.
    /// </summary>
    [Parameter]
    public double Min { get; set; } = 0;

    /// <summary>
    /// Maximum value of the slider.
    /// </summary>
    [Parameter]
    public double Max { get; set; } = 100;

    /// <summary>
    /// Step increment for the value.
    /// </summary>
    [Parameter]
    public double Step { get; set; } = 1;

    /// <summary>
    /// Size of the slider (affects track and thumb dimensions).
    /// </summary>
    [Parameter]
    public Size Size { get; set; } = Size.Md;

    /// <summary>
    /// Whether the slider is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// ARIA label for accessibility.
    /// </summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Slider";

    // Canonical list of values, resolved from either Value or Values
    private List<double> _localValues = [0];
    private int _activeThumbIndex = 0;
    private bool _isDragging = false;
    private ElementReference _containerRef;
    private IJSObjectReference? _module;
    private string _elementId = $"slider-{Guid.NewGuid()}";

    private bool _isSingleMode => Values == null;

    /// <summary>
    /// Track height in pixels based on size.
    /// </summary>
    private int TrackHeightPx => Size switch
    {
        Size.Sm => 2,
        Size.Md => 4,
        Size.Lg => 6,
        _ => 4,
    };

    /// <summary>
    /// Thumb size in pixels based on size.
    /// </summary>
    private int ThumbSizePx => Size switch
    {
        Size.Sm => 8,
        Size.Md => 12,
        Size.Lg => 16,
        _ => 12,
    };

    /// <summary>
    /// Returns CSS classes for the slider container.
    /// </summary>
    private string GetContainerClass()
    {
        var baseClasses = Size switch
        {
            Size.Sm => "relative w-full h-4 select-none",
            Size.Md => "relative w-full h-5 select-none",
            Size.Lg => "relative w-full h-6 select-none",
            _ => "relative w-full h-5 select-none",
        };

        var stateClasses = Disabled ? "opacity-50" : string.Empty;
        return MergeCss(baseClasses, stateClasses, Class);
    }

    /// <summary>
    /// Returns inline style for the track.
    /// </summary>
    private string GetTrackStyle() => $"height: {TrackHeightPx}px;";

    /// <summary>
    /// Returns inline style for the filled range segment.
    /// </summary>
    private string GetRangeStyle(double startPercent, double endPercent)
    {
        var left = Math.Min(startPercent, endPercent);
        var width = Math.Abs(endPercent - startPercent);
        return $"height: {TrackHeightPx}px; left: {left:0.##}%; width: {width:0.##}%;";
    }

    /// <summary>
    /// Returns inline style for the thumb at the given percent position.
    /// </summary>
    private string GetThumbStyle(double percent)
    {
        var halfThumb = ThumbSizePx / 2.0;
        return $"width: {ThumbSizePx}px; height: {ThumbSizePx}px; left: calc({percent:0.##}% - {halfThumb:0.##}px);";
    }

    /// <summary>
    /// Clamps a value to the Min/Max range.
    /// </summary>
    private double ClampValue(double value)
    {
        if (Max <= Min)
        {
            return Min;
        }

        return Math.Clamp(value, Min, Max);
    }

    /// <summary>
    /// Converts a value to a percentage (0-100) within the Min/Max range.
    /// </summary>
    private double ToPercent(double value)
    {
        var range = Max - Min;
        if (range <= 0)
        {
            return 0;
        }

        var clamped = ClampValue(value);
        return ((clamped - Min) / range) * 100;
    }

    /// <summary>
    /// Resolves _localValues from either Values (multi-mode) or Value (single-mode).
    /// Precedence: Values > Value > default [0]
    /// </summary>
    protected override void OnParametersSet()
    {
        if (Values != null)
        {
            // Multi-thumb mode: clamp all values
            _localValues = Values.Select(ClampValue).ToList();
        }
        else
        {
            // Single-thumb mode: clamp the single value
            _localValues = [ClampValue(Value)];
        }
    }

    /// <summary>
    /// Converts pointer position (0-1) to slider value.
    /// </summary>
    private double GetValueFromPointerPercent(double percent)
    {
        percent = Math.Clamp(percent, 0, 100);
        return Min + (percent / 100) * (Max - Min);
    }

    /// <summary>
    /// Finds the index of the closest thumb to a given value.
    /// Used when clicking on the track to determine which thumb to move.
    /// </summary>
    private int GetClosestThumbIndex(double value)
    {
        if (_localValues.Count == 1)
            return 0;

        var distances = _localValues.Select(v => Math.Abs(v - value)).ToList();
        var minDistance = distances.Min();
        return distances.IndexOf(minDistance);
    }

    /// <summary>
    /// Handles pointer down on the track - selects the closest thumb.
    /// Implements Radix's "closest thumb" selection behavior.
    /// </summary>
    private async Task OnTrackPointerDown(PointerEventArgs e)
    {
        if (Disabled)
            return;

        // Calculate which percentage along the track was clicked
        var percent = await GetPointerPercent(e.ClientX);
        var clickedValue = GetValueFromPointerPercent(percent);

        // Find and activate the closest thumb
        _activeThumbIndex = GetClosestThumbIndex(clickedValue);

        // Move the closest thumb to the clicked position
        _isDragging = true;
        await UpdateThumbValue(_activeThumbIndex, clickedValue);
    }

    /// <summary>
    /// Handles thumb pointer down - marks it as active for dragging.
    /// </summary>
    private async Task OnThumbPointerDown(int index, PointerEventArgs e)
    {
        if (Disabled)
            return;

        _activeThumbIndex = index;
        _isDragging = true;

        // Capture pointer so drags work even outside the slider
        try
        {
            if (_module is not null)
            {
                await _module.InvokeVoidAsync("setPointerCapture", _elementId, e.PointerId);
            }
        }
        catch { }
    }

    /// <summary>
    /// Handles keyboard input on the thumb - left/right arrow keys move the slider.
    /// </summary>
    private async Task OnThumbKeyDown(int index, KeyboardEventArgs e)
    {
        if (Disabled)
            return;

        double newValue = _localValues[index];

        if (e.Key == "ArrowLeft")
        {
            newValue -= Step;
            await UpdateThumbValue(index, newValue);
        }
        else if (e.Key == "ArrowRight")
        {
            newValue += Step;
            await UpdateThumbValue(index, newValue);
        }
    }

    /// <summary>
    /// Handles pointer move during dragging - updates the active thumb value.
    /// </summary>
    private async Task OnPointerMove(PointerEventArgs e)
    {
        if (!_isDragging || Disabled)
            return;

        var percent = await GetPointerPercent(e.ClientX);
        var newValue = GetValueFromPointerPercent(percent);
        await UpdateThumbValue(_activeThumbIndex, newValue);
    }

    /// <summary>
    /// Handles pointer up - stops dragging and releases pointer capture.
    /// </summary>
    private async Task OnPointerUp(PointerEventArgs e)
    {
        _isDragging = false;

        // Release pointer capture
        try
        {
            if (_module is not null)
            {
                await _module.InvokeVoidAsync("releasePointerCapture", _elementId, e.PointerId);
            }
        }
        catch { }
    }

    /// <summary>
    /// Load the scoped JS module.
    /// </summary>
    private async Task LoadModuleAsync()
    {
        try
        {
            var assemblyName = typeof(Slider).Assembly.GetName().Name;
            _module = await JS.InvokeAsync<IJSObjectReference>(
                "import", $"./_content/{assemblyName}/Components/Slider/Slider.razor.js"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load Slider module: {ex.Message}");
        }
    }

    /// <summary>
    /// Initialize on first render.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadModuleAsync();
        }
    }

    /// <summary>
    /// Gets pointer position as a percentage (0-100) along the slider track.
    /// Uses JavaScript interop to calculate position relative to the container.
    /// </summary>
    private async Task<double> GetPointerPercent(double clientX)
    {
        try
        {
            _module ??= await LoadModuleAsync().ContinueWith(_ => _module);

            if (_module is null)
                return 50.0;

            var percent = await _module.InvokeAsync<double>(
                "getPointerPercentage", _elementId, clientX);

            return Math.Clamp(percent, 0, 100);
        }
        catch
        {
            return 50.0;
        }
    }

    /// <summary>
    /// Dispose JS module on component disposal.
    /// </summary>
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_module is not null)
        {
            try
            {
                await _module.DisposeAsync();
            }
            catch { }
        }
    }

    /// <summary>
    /// Updates a thumb's value and invokes callbacks.
    /// Allows adjacent thumbs to swap positions.
    /// </summary>
    private async Task UpdateThumbValue(int index, double newValue)
    {
        newValue = ClampValue(newValue);

        // Update the value at the index
        var oldValues = new List<double>(_localValues);
        _localValues[index] = newValue;

        // Sort values so they stay in order, but track which index this value moved to
        var sortedValues = _localValues.OrderBy(v => v).ToList();
        _activeThumbIndex = sortedValues.IndexOf(newValue);
        _localValues = sortedValues;

        // Invoke appropriate callback
        if (_isSingleMode)
        {
            Value = _localValues[0];
            await ValueChanged.InvokeAsync(Value);
        }
        else
        {
            await ValuesChanged.InvokeAsync(_localValues);
        }
    }

    /// <summary>
    /// Calculates the style for the filled range (from first to last thumb).
    /// </summary>
    private string GetRangeStyle()
    {
        if (_localValues.Count == 0)
            return GetRangeStyle(0, 0);

        if (_isSingleMode)
        {
            // Single mode: fill from 0 to value percent
            return GetRangeStyle(0, ToPercent(_localValues[0]));
        }

        // Multi-thumb: fill from min to max thumb
        var minPercent = ToPercent(_localValues.Min());
        var maxPercent = ToPercent(_localValues.Max());
        return GetRangeStyle(minPercent, maxPercent);
    }
}
