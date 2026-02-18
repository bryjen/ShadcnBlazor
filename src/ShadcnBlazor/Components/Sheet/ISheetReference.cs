using ShadcnBlazor.Components.Sheet.Models;

namespace ShadcnBlazor.Components.Sheet;

/// <summary>
/// Reference to a shown sheet, used to await its result and close it programmatically.
/// </summary>
public interface ISheetReference
{
    /// <summary>
    /// Unique identifier of the sheet.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Task that completes when the sheet is closed.
    /// </summary>
    Task<SheetResult> Result { get; }

    /// <summary>
    /// Closes the sheet with the specified result.
    /// </summary>
    void Close(SheetResult result);
}
