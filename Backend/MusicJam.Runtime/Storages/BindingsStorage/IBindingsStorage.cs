using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Storages.BindingsStorage;

public interface IBindingsStorage
{
    Task<Binding> AddOrUpdate(Binding binding);

    Task<Binding?> GetBinding(Guid sessionId);

    Task<Binding?> RemoveConnection(string connectionId);

    Task<Binding?> DeleteBinding(Guid sessionId);
}
