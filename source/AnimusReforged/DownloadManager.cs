using AnimusReforged.Paths;

namespace AnimusReforged;

public class DownloadManager
{
    // Variables
    private static readonly HttpClient _httpClient = new HttpClient{ Timeout = TimeSpan.FromSeconds(60) };

    public event Action<int>? ProgressChanged;
    
    // Constructor
    public DownloadManager()
    {
        // Set the User-Agent header once for all requests.
        if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Xenia Manager (https://github.com/xenia-manager/xenia-manager)");
        }
    
        // Ensure the default directory exists
        Directory.CreateDirectory(AppPaths.Downloads);
    }
    
    public async Task DownloadFileAsync(string url, string savePath, CancellationToken cancellationToken = default)
    {
        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();
            long totalBytes = response.Content.Headers.ContentLength ?? -1L;
            long downloadedBytes = 0;

            await using FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await using Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            byte[] buffer = new byte[8192];
            int bytesRead;
            while ((bytesRead = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                downloadedBytes += bytesRead;
                UpdateProgress(downloadedBytes, totalBytes);
            }
        }
        catch (TaskCanceledException)
        {
            Logger.Warning("Download was cancelled.");
            throw;
        }
        catch (HttpRequestException hrex)
        {
            Logger.Error($"HTTP error during download: {hrex}");
            throw;
        }
        catch (Exception ex)
        {
            Logger.Error($"An error occurred during download: {ex}");
            throw;
        }
    }
    
    private void UpdateProgress(long downloadedBytes, long totalBytes)
    {
        if (totalBytes > 0)
        {
            int progress = (int)Math.Round((double)downloadedBytes / totalBytes * 100);
            ProgressChanged?.Invoke(progress);
        }
    }
}