using ShadcnBlazor.Components.Sheet.Models;
using ShadcnBlazor.Components.Sheet.Services;

namespace ShadcnBlazor.Components.Sheet;

/// <summary>
/// Reference to a shown sheet.
/// </summary>
public sealed class SheetReference : ISheetReference
{
    private readonly SheetService _sheetService;

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public Task<SheetResult> Result =>
        _sheetService.SheetInstances
            .FirstOrDefault(x => x.Id == Id)
            ?.TaskCompletionSource.Task
        ?? Task.FromResult(SheetResult.Cancel());

    /// <summary>
    /// Initializes a new instance of the <see cref="SheetReference"/> class.
    /// </summary>
    public SheetReference(Guid id, SheetService sheetService)
    {
        Id = id;
        _sheetService = sheetService;
    }

    /// <inheritdoc />
    public void Close(SheetResult result)
    {
        _ = _sheetService.CloseAsync(this, result);
    }

    /// <summary>
    /// Completes the close after the container has run its close sequence.
    /// </summary>
    internal void CompleteClose(SheetResult result)
    {
        _sheetService.CompleteClose(this, result);
    }
}
