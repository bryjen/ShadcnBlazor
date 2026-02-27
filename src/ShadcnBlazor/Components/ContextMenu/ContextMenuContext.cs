namespace ShadcnBlazor.Components.ContextMenu;

/// <summary>
/// Shared context for ContextMenu and its child components.
/// </summary>
public class ContextMenuContext
{
    /// <summary>Whether the context menu is currently open.</summary>
    public bool Open { get; private set; }

    /// <summary>The X position (clientX) where the context menu was triggered.</summary>
    public double X { get; private set; }

    /// <summary>The Y position (clientY) where the context menu was triggered.</summary>
    public double Y { get; private set; }

    /// <summary>Opens the menu at the given cursor coordinates.</summary>
    public Func<double, double, Task>? SetOpen { get; set; }

    /// <summary>Closes the context menu.</summary>
    public Func<Task>? Close { get; set; }

    /// <summary>Called internally to update open state and position.</summary>
    internal void OpenAt(double x, double y)
    {
        X = x;
        Y = y;
        Open = true;
    }

    /// <summary>Called internally to close.</summary>
    internal void CloseMenu()
    {
        Open = false;
    }
}
