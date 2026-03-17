using ShadcnBlazor.Components.Sheet.Models;

namespace ShadcnBlazor.Components.Sheet;

/// <summary>
/// Interface cascaded to sheet content components, allowing them to close the sheet.
/// </summary>
public interface ISheetInstance
{
    /// <summary>
    /// Closes the sheet with the specified result.
    /// </summary>
    void Close(SheetResult result);

    /// <summary>
    /// Closes the sheet with a successful result.
    /// </summary>
    void Close();

    /// <summary>
    /// Cancels the sheet.
    /// </summary>
    void Cancel();
}
