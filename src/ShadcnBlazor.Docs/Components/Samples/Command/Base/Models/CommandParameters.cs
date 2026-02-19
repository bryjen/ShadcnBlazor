using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Dialog.Models;

namespace ShadcnBlazor.Docs.Components.Samples.Command.Base.Models;

/// <summary>
/// Typed parameters for <see cref="CommandDialog{T}"/>. Extends <see cref="DialogParameters"/>.
/// </summary>
public class CommandParameters<T> : DialogParameters
{
    /// <summary>
    /// The items to display and filter in the command dialog.
    /// </summary>
    public IReadOnlyList<T> CommandItems
    {
        get => (IReadOnlyList<T>?)this[nameof(CommandItems)] ?? [];
        set => this[nameof(CommandItems)] = value;
    }

    /// <summary>
    /// Filters items by search query. Receives all items and the search string; returns filtered items.
    /// </summary>
    public Func<IReadOnlyList<T>, string, IReadOnlyList<T>> Filter
    {
        get => (Func<IReadOnlyList<T>, string, IReadOnlyList<T>>?)this[nameof(Filter)]!;
        set => this[nameof(Filter)] = value;
    }

    /// <summary>
    /// Renders the filtered items. Receives the filtered list; returns a <see cref="RenderFragment"/>.
    /// </summary>
    public Func<IReadOnlyList<T>, int?, RenderFragment> RenderCommandItems
    {
        get => (Func<IReadOnlyList<T>, int?, RenderFragment>?)this[nameof(RenderCommandItems)]!;
        set => this[nameof(RenderCommandItems)] = value;
    }
}
