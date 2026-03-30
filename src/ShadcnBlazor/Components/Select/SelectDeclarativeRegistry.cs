using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Components.Select;

/// <summary>Node kinds supported by declarative select content.</summary>
public enum SelectNodeKind
{
    /// <summary>Selectable item.</summary>
    Item,
    /// <summary>Non-selectable label.</summary>
    Label,
    /// <summary>Visual separator.</summary>
    Separator,
    /// <summary>Action row (non-value item).</summary>
    Action,
}

/// <summary>Flattened representation of a declarative select node.</summary>
public sealed class SelectDeclarativeNode
{
    /// <summary>Internal id assigned by the registry.</summary>
    public int Id { get; init; }
    /// <summary>Node kind.</summary>
    public SelectNodeKind Kind { get; init; }
    /// <summary>Associated value for items.</summary>
    public object? Value { get; set; }
    /// <summary>Display text.</summary>
    public string Text { get; set; } = string.Empty;
    /// <summary>Whether the node is disabled.</summary>
    public bool Disabled { get; set; }
    /// <summary>Optional render fragment for custom output.</summary>
    public RenderFragment? Render { get; set; }
    /// <summary>Optional callback for action nodes.</summary>
    public Func<Task>? Callback { get; set; }
}

/// <summary>Registry that tracks declarative select nodes.</summary>
public sealed class SelectDeclarativeRegistry
{
    private readonly List<SelectDeclarativeNode> _nodes = [];
    private int _nextId = 1;

    /// <summary>Raised when the registry contents change.</summary>
    public event Action? Changed;
    
    /// <summary>Registers an item node and returns its id.</summary>
    public int RegisterItem(object? value, string text, bool disabled, RenderFragment? render = null)
    {
        var id = _nextId++;
        _nodes.Add(new SelectDeclarativeNode
        {
            Id = id,
            Kind = SelectNodeKind.Item,
            Value = value,
            Text = text,
            Disabled = disabled,
            Render = render
        });
        Changed?.Invoke();
        return id;
    }

    /// <summary>Registers a label node and returns its id.</summary>
    public int RegisterLabel(string text)
    {
        var id = _nextId++;
        _nodes.Add(new SelectDeclarativeNode
        {
            Id = id,
            Kind = SelectNodeKind.Label,
            Text = text
        });
        Changed?.Invoke();
        return id;
    }

    /// <summary>Registers a separator node and returns its id.</summary>
    public int RegisterSeparator()
    {
        var id = _nextId++;
        _nodes.Add(new SelectDeclarativeNode
        {
            Id = id,
            Kind = SelectNodeKind.Separator
        });
        Changed?.Invoke();
        return id;
    }

    /// <summary>Updates an existing item node.</summary>
    public void UpdateItem(int id, object? value, string text, bool disabled, RenderFragment? render = null)
    {
        var node = _nodes.FirstOrDefault(x => x.Id == id && x.Kind == SelectNodeKind.Item);
        if (node is null)
            return;

        if (Equals(node.Value, value)
            && string.Equals(node.Text, text, StringComparison.Ordinal)
            && node.Disabled == disabled)
            return;

        node.Value = value;
        node.Text = text;
        node.Disabled = disabled;
        Changed?.Invoke();
    }

    /// <summary>Updates an existing label node.</summary>
    public void UpdateLabel(int id, string text)
    {
        var node = _nodes.FirstOrDefault(x => x.Id == id && x.Kind == SelectNodeKind.Label);
        if (node is null)
            return;

        if (string.Equals(node.Text, text, StringComparison.Ordinal))
            return;

        node.Text = text;
        Changed?.Invoke();
    }

    /// <summary>Removes a node by id.</summary>
    public void Remove(int id)
    {
        var removed = _nodes.RemoveAll(x => x.Id == id) > 0;
        if (removed)
        {
            Changed?.Invoke();
        }
    }

    /// <summary>Returns a snapshot of the current nodes.</summary>
    public IReadOnlyList<SelectDeclarativeNode> Snapshot() => [.. _nodes];
}
