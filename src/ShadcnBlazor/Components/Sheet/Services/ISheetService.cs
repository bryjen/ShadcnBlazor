using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Sheet.Models;

namespace ShadcnBlazor.Components.Sheet.Services;

/// <summary>
/// Service for imperative sheet display.
/// </summary>
public interface ISheetService
{
    /// <summary>
    /// Shows a sheet with the specified component type.
    /// </summary>
    /// <typeparam name="T">The component type to render.</typeparam>
    /// <param name="parameters">Parameters to pass to the component.</param>
    /// <returns>Reference to the shown sheet.</returns>
    ISheetReference Show<T>(SheetParameters? parameters = null)
        where T : ComponentBase;
}
