using ShadcnBlazor.Components.Sheet.Models;

namespace ShadcnBlazor.Components.Sheet;

/// <summary>
/// Concrete implementation of <see cref="ISheetInstance"/> cascaded to sheet content.
/// </summary>
internal sealed class SheetInstanceCascade : ISheetInstance
{
    private readonly Action<SheetResult> _close;

    public SheetInstanceCascade(Action<SheetResult> close)
    {
        _close = close;
    }

    /// <inheritdoc />
    public void Close(SheetResult result)
    {
        _close(result);
    }

    /// <inheritdoc />
    public void Close()
    {
        _close(SheetResult.Ok());
    }

    /// <inheritdoc />
    public void Cancel()
    {
        _close(SheetResult.Cancel());
    }
}
