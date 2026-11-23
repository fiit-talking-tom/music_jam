using System.Collections.Concurrent;
using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Storages.BindingsStorage;

public sealed class InMemoryBindingsStorage : IBindingsStorage
{
    private readonly ConcurrentDictionary<Guid, Binding> _bindings = new();

    public async Task<Binding> AddOrUpdate(Binding binding)
    {
        await Task.CompletedTask;
        _bindings[binding.SessionId] = binding;
        return binding;
    }

    public async Task<Binding?> GetBinding(Guid sessionId)
    {
        await Task.CompletedTask;
        return _bindings.GetValueOrDefault(sessionId);
    }

    public async Task<Binding?> RemoveConnection(string connectionId)
    {
        await Task.CompletedTask;

        var binding = _bindings
            .Select(b => b.Value)
            .FirstOrDefault(b => b.Connections.Contains(connectionId));

        if (binding is null)
        {
            return null;
        }

        binding.Connections.Remove(connectionId);

        return binding;
    }

    public async Task<Binding?> DeleteBinding(Guid sessionId)
    {
        await Task.CompletedTask;
        return _bindings.TryRemove(sessionId, out var binding) ? binding : null;
    }
}
