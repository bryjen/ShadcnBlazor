namespace ShadcnBlazor.Docs.Services;

public record TocItem(string Title, string Id, int Level = 0);

public class PageTocService
{
    private List<TocItem> _items = [];
    private event Action? _changed;

    public IReadOnlyList<TocItem> Items => _items;

    public void SetItems(IEnumerable<TocItem> items)
    {
        _items = items.ToList();
        _changed?.Invoke();
    }

    public void Clear()
    {
        _items = [];
    }

    public void AddSection(string title, string id, int level = 0)
    {
        _items.Add(new TocItem(title, id, level));
    }

    public void Flush()
    {
        _changed?.Invoke();
    }

    public void Subscribe(Action callback)
    {
        _changed += callback;
    }

    public void Unsubscribe(Action callback)
    {
        _changed -= callback;
    }
}
