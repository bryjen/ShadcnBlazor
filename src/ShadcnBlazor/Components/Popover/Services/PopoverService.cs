using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Popover.Models;

namespace ShadcnBlazor.Components.Popover.Services;

/// <summary>
/// Implementation of <see cref="IPopoverService"/> for popover JavaScript interop.
/// </summary>
public class PopoverService : IPopoverService, IAsyncDisposable
{
    private readonly PopoverInterop _popoverInterop;
    private readonly PopoverOptions _options;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _isInitialized;
    private int? _repositionDebounceOverride;

    /// <summary>
    /// Creates a new PopoverService.
    /// </summary>
    /// <param name="popoverInterop">The popover JavaScript interop.</param>
    /// <param name="options">Popover configuration options.</param>
    public PopoverService(PopoverInterop popoverInterop, IOptions<PopoverOptions> options)
    {
        _popoverInterop = popoverInterop;
        _options = options.Value;
    }

    /// <inheritdoc />
    public async Task InitializeAsync(string containerClass, int flipMargin, int overflowPadding, int baseZIndex)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (!_isInitialized)
            {
                await _popoverInterop.InitializeAsync(containerClass, flipMargin, overflowPadding, baseZIndex);
                _isInitialized = true;
            }

            if (_repositionDebounceOverride.HasValue)
            {
                await _popoverInterop.SetRepositionDebounceAsync(_repositionDebounceOverride.Value);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task SetRepositionDebounceAsync(int debounceMilliseconds)
    {
        var normalized = Math.Max(0, debounceMilliseconds);
        _repositionDebounceOverride = normalized;

        if (_isInitialized)
        {
            await _popoverInterop.SetRepositionDebounceAsync(normalized);
        }
    }

    /// <inheritdoc />
    public async Task EnableOutsideClickCloseAsync(string anchorId, string popoverId, DotNetObjectReference<Popover> callbackReference)
    {
        await InitializeAsync(
            _options.ContainerClass,
            _options.FlipMargin,
            _options.OverflowPadding,
            _options.BaseZIndex);

        await _popoverInterop.EnableOutsideClickCloseAsync(anchorId, popoverId, callbackReference);
    }

    /// <inheritdoc />
    public async Task DisableOutsideClickCloseAsync(string popoverId)
    {
        if (!_isInitialized)
        {
            return;
        }

        await _popoverInterop.DisableOutsideClickCloseAsync(popoverId);
    }

    /// <inheritdoc />
    public async Task ConnectAsync(string anchorId, string popoverId)
    {
        await InitializeAsync(
            _options.ContainerClass,
            _options.FlipMargin,
            _options.OverflowPadding,
            _options.BaseZIndex);

        await _popoverInterop.ConnectAsync(anchorId, popoverId);
    }

    /// <inheritdoc />
    public async Task DisconnectAsync(string popoverId)
    {
        if (!_isInitialized)
        {
            return;
        }

        await _popoverInterop.DisconnectAsync(popoverId);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (!_isInitialized)
        {
            return;
        }

        try
        {
            await _popoverInterop.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
            // Expected during circuit shutdown.
        }

        _semaphore.Dispose();
    }
}
