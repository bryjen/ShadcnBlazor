using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared.Services.Interop;

namespace ShadcnBlazor.Components.Shared.Services;

/// <summary>
/// Implementation of <see cref="ISheetJsService"/> for sheet JavaScript interop.
/// </summary>
public class SheetJsService : ISheetJsService
{
    private readonly SheetInterop _sheetInterop;

    /// <summary>
    /// Creates a new SheetJsService.
    /// </summary>
    /// <param name="sheetInterop">The sheet JavaScript interop.</param>
    public SheetJsService(SheetInterop sheetInterop)
    {
        _sheetInterop = sheetInterop;
    }

    /// <inheritdoc />
    public ValueTask InitializeAsync<T>(string sheetId, DotNetObjectReference<T> dotNetRef, CancellationToken cancellationToken = default) where T : class
        => _sheetInterop.InitializeAsync(sheetId, dotNetRef, cancellationToken);

    /// <inheritdoc />
    public ValueTask OpenAsync(string sheetId, CancellationToken cancellationToken = default)
        => _sheetInterop.OpenAsync(sheetId, cancellationToken);

    /// <inheritdoc />
    public ValueTask CloseAsync(string sheetId, CancellationToken cancellationToken = default)
        => _sheetInterop.CloseAsync(sheetId, cancellationToken);

    /// <inheritdoc />
    public ValueTask DisposeSheetAsync(string sheetId, CancellationToken cancellationToken = default)
        => _sheetInterop.DisposeSheetAsync(sheetId, cancellationToken);

    /// <inheritdoc />
    public ValueTask FocusFirstInSheetAsync(string sheetId, CancellationToken cancellationToken = default)
        => _sheetInterop.FocusFirstInSheetAsync(sheetId, cancellationToken);
}
