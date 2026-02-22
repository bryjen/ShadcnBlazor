using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Components.Select;

internal enum SelectNodeKind
{
    Item,
    Label,
    Separator,
}

internal sealed class SelectDeclarativeNode
{
    public int Id { get; init; }
    public SelectNodeKind Kind { get; init; }
    public object? Value { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool Disabled { get; set; }
    
    public RenderFragment? Render { get; set; }
}

internal sealed class SelectDeclarativeRegistry
{
    private readonly List<SelectDeclarativeNode> _nodes = [];
    private int _nextId = 1;

    public event Action? Changed;
    
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

    public void UpdateItem(int id, object? value, string text, bool disabled, RenderFragment? render = null)
    {
        var node = _nodes.FirstOrDefault(x => x.Id == id && x.Kind == SelectNodeKind.Item);
        if (node is null)
            return;

        if (Equals(node.Value, value) && string.Equals(node.Text, text, StringComparison.Ordinal) && node.Disabled == disabled)
            return;

        node.Value = value;
        node.Text = text;
        node.Disabled = disabled;
        node.Render = render;
        Changed?.Invoke();
    }

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

    public void Remove(int id)
    {
        var removed = _nodes.RemoveAll(x => x.Id == id) > 0;
        if (removed)
        {
            Changed?.Invoke();
        }
    }

    public IReadOnlyList<SelectDeclarativeNode> Snapshot() => [.. _nodes];
}
