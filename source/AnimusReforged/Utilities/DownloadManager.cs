using AnimusReforged.Logging;

namespace AnimusReforged.Utilities;

/// <summary>
/// Manages file downloads with progress reporting and error handling.
/// </summary>
public class DownloadManager : IDisposable
{
    private readonly string _downloadPath = FilePaths.DownloadsDirectory;
    private readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
    private bool _disposed = false;

    /// <summary>
    /// Occurs when the download progress changes.
    /// </summary>
    public event Action<int>? ProgressChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadManager"/> class.
    /// Sets up the HTTP client with a custom User-Agent and ensures the download directory exists.
    /// </summary>
    public DownloadManager()
    {
        // Set the User-Agent header once for all requests.
        if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Animus Reforged (https://github.com/animus-reforged)");
        }

        // Ensure the default directory exists
        Directory.CreateDirectory(_downloadPath);
    }

    /// <summary>
    /// Downloads a file from the specified URL to the given save path asynchronously.
    /// </summary>
    /// <param name="url">The URL of the file to download.</param>
    /// <param name="savePath">The local path where the file should be saved.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the download operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when an HTTP error occurs during download.</exception>
    /// <exception cref="TaskCanceledException">Thrown when the download is cancelled.</exception>
    public async Task DownloadFileAsync(string url, string savePath, CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL cannot be null or empty.", nameof(url));
        }

        if (string.IsNullOrWhiteSpace(savePath))
        {
            throw new ArgumentException("Save path cannot be null or empty.", nameof(savePath));
        }

        Directory.CreateDirectory(_downloadPath);

        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();
            long totalBytes = response.Content.Headers.ContentLength ?? -1L;
            long downloadedBytes = 0;

            // Create the directory for the save path if it doesn't exist
            string? directoryPath = Path.GetDirectoryName(savePath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            await using FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 8192, useAsync: true);
            await using Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);

            byte[] buffer = new byte[8192];
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                downloadedBytes += bytesRead;
                UpdateProgress(downloadedBytes, totalBytes);
            }

            // Ensure the stream is properly flushed
            await fileStream.FlushAsync(cancellationToken);

            // Report 100% completion only on successful download
            ProgressChanged?.Invoke(100);

            Logger.Info<DownloadManager>($"Successfully downloaded file from {url} to {savePath}");
        }
        catch (TaskCanceledException)
        {
            Logger.Warning<DownloadManager>($"Download was cancelled for URL: {url}");
            throw;
        }
        catch (HttpRequestException hrex)
        {
            Logger.Error<DownloadManager>($"HTTP error during download from {url}: {hrex.Message}");
            throw;
        }
        catch (IOException ioex)
        {
            Logger.Error<DownloadManager>($"IO error during download to {savePath}: {ioex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Logger.Error<DownloadManager>($"An unexpected error occurred during download from {url}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Updates the download progress by calculating the percentage based on downloaded and total bytes.
    /// </summary>
    /// <param name="downloadedBytes">The number of bytes downloaded so far.</param>
    /// <param name="totalBytes">The total number of bytes to download, or -1 if unknown.</param>
    private void UpdateProgress(long downloadedBytes, long totalBytes)
    {
        if (totalBytes > 0)
        {
            int progress = (int)Math.Round((double)downloadedBytes / totalBytes * 100);
            ProgressChanged?.Invoke(progress);
        }
    }

    /// <summary>
    /// Disposes of the resources used by the DownloadManager.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the DownloadManager and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }
}