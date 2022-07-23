using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.Services;

internal class ImageService : IImageStorage
{
    private readonly string _baseDirectory;
    private readonly string _coreDirectory;
    private readonly string[] _supportedFormats;

    public ImageService(string baseDirectory, string coreDirectory, string[] supportedFormats)
    {
        _baseDirectory = baseDirectory;
        _coreDirectory = coreDirectory;
        _supportedFormats = supportedFormats;
    }

    public Task<string> GetImageAsync(string name, string directory = null) => Task.FromResult(Path.Combine(_baseDirectory, string.IsNullOrWhiteSpace(directory) ? _coreDirectory : directory, name));

    public Task<string> SaveImageAsync(string url, string directory)
    {
        var directoryPath = Path.Combine(_baseDirectory, directory);
        var imageName = Guid.NewGuid();
        return DownloadImageAsync(directoryPath, imageName.ToString(), new Uri(url));
    }

    private async Task<string> DownloadImageAsync(string directoryPath, string fileName, Uri uri)
    {
        var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
        var fileExtension = Path.GetExtension(uriWithoutQuery);
        if (!_supportedFormats.Contains(fileExtension, StringComparer.CurrentCultureIgnoreCase))
            throw new FormatException("The provided image is not of a supported type.");

        var path = Path.Combine(directoryPath, fileName + fileExtension);
        Directory.CreateDirectory(directoryPath);

        using var httpClient = new HttpClient();
        var imageBytes = await httpClient.GetByteArrayAsync(uri).ConfigureAwait(false);
        await File.WriteAllBytesAsync(path, imageBytes).ConfigureAwait(false);

        return path;
    }

    public Task DeleteImageAsync(string path)
    {
        var file = new FileInfo(path);
        if (!file.Exists) return Task.CompletedTask;
        if (!string.IsNullOrEmpty(file.Directory?.Name) && file.Directory.Name.Equals(_coreDirectory, StringComparison.CurrentCultureIgnoreCase)) return Task.CompletedTask;
            
        file.Delete();
        return Task.CompletedTask;
    }
}