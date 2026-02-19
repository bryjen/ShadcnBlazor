using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Shared.Services;

/// <summary>
/// Service for sheet JavaScript interop (initialize, open, close, dispose, focus).
/// </summary>
public interface ISheetJsService
{
    /// <summary>Initializes the sheet and registers the .NET callback reference.</summary>
    ValueTask InitializeAsync<T>(string sheetId, DotNetObjectReference<T> dotNetRef, CancellationToken cancellationToken = default) where T : class;

    /// <summary>Opens the sheet and triggers the open animation.</summary>
    ValueTask OpenAsync(string sheetId, CancellationToken cancellationToken = default);

    /// <summary>Closes the sheet and triggers the close animation.</summary>
    ValueTask CloseAsync(string sheetId, CancellationToken cancellationToken = default);

    /// <summary>Disposes the sheet instance and cleans up event listeners.</summary>
    ValueTask DisposeSheetAsync(string sheetId, CancellationToken cancellationToken = default);

    /// <summary>Moves focus to the first focusable element inside the sheet.</summary>
    ValueTask FocusFirstInSheetAsync(string sheetId, CancellationToken cancellationToken = default);
}
