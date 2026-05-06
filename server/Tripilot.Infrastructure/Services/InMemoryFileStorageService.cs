using Tripilot.Application.Common.Interfaces;

namespace Tripilot.Infrastructure.Services;

/// <summary>
/// Development stub file storage: stores metadata only, returns synthetic URLs.
/// </summary>
public class InMemoryFileStorageService : IFileStorageService
{
    private readonly Dictionary<string, byte[]> _store = new();

    public async Task<string> UploadAsync(Stream stream, string fileName, string folder, CancellationToken cancellationToken = default)
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, cancellationToken);
        var id = $"{folder}/{Guid.NewGuid()}-{fileName}";
        _store[id] = ms.ToArray();
        return $"https://dev-storage.local/{id}";
    }

    public Task DeleteAsync(string fileUrlOrId, CancellationToken cancellationToken = default)
    {
        var key = fileUrlOrId.Replace("https://dev-storage.local/", string.Empty);
        _store.Remove(key);
        return Task.CompletedTask;
    }
}
