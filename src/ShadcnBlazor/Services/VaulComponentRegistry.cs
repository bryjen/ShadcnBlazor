using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;

namespace ShadcnBlazor.Services;

/// <summary>
/// Registry for RenderFragments used by Vaul component drawers.
/// </summary>
public sealed class VaulComponentRegistry
{
    private readonly ConcurrentDictionary<string, RenderFragment> _fragments = new();

    /// <summary>
    /// Registers a fragment and returns its unique ID.
    /// </summary>
    public string Register(RenderFragment fragment)
    {
        var id = $"vaul_{Guid.NewGuid():N}";
        _fragments[id] = fragment;
        return id;
    }

    /// <summary>
    /// Tries to get a fragment by ID.
    /// </summary>
    public RenderFragment? TryGet(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        return _fragments.TryGetValue(id, out var fragment) ? fragment : null;
    }

    /// <summary>
    /// Removes a fragment by ID.
    /// </summary>
    public void Remove(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return;
        }

        _fragments.TryRemove(id, out _);
    }
}
