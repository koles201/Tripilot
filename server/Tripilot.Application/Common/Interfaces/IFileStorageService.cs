namespace Tripilot.Application.Common.Interfaces;

/// <summary>
/// Abstraction for storing files (documents, images) and retrieving public URLs.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Stores a file stream and returns its accessible URL or identifier.
    /// </summary>
    /// <param name="stream">File data stream.</param>
    /// <param name="fileName">Original file name.</param>
    /// <param name="folder">Logical folder/category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<string> UploadAsync(Stream stream, string fileName, string folder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a previously stored file by URL or identifier.
    /// </summary>
    Task DeleteAsync(string fileUrlOrId, CancellationToken cancellationToken = default);
}
