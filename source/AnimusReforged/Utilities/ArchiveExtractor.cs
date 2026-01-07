using AnimusReforged.Logging;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

namespace AnimusReforged.Utilities;

/// <summary>
/// Provides methods for extracting files from various archive formats including ZIP and RAR.
/// </summary>
public class ArchiveExtractor
{
    /// <summary>
    /// Extracts all files or specific files from a ZIP archive to the specified output directory.
    /// </summary>
    /// <param name="zipPath">The path to the ZIP archive to extract.</param>
    /// <param name="outputPath">The directory where files should be extracted.</param>
    /// <param name="filesToExtract">Optional array of specific files to extract. If null or empty, all files are extracted.</param>
    /// <exception cref="ArgumentException">Thrown when zipPath or outputPath is null, empty, or whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified ZIP file does not exist.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the output directory does not exist and cannot be created.</exception>
    /// <exception cref="InvalidFormatException">Thrown when the archive format is invalid.</exception>
    private static void ExtractZip(string zipPath, string outputPath, string[]? filesToExtract = null)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(zipPath))
        {
            throw new ArgumentException("ZIP path cannot be null or empty.", nameof(zipPath));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path cannot be null or empty.", nameof(outputPath));
        }

        if (!File.Exists(zipPath))
        {
            throw new FileNotFoundException($"ZIP file does not exist: {zipPath}");
        }

        try
        {
            Directory.CreateDirectory(outputPath);

            using IArchive archive = ArchiveFactory.Open(zipPath);
            int extractedCount = 0;

            foreach (IArchiveEntry entry in archive.Entries.Where(entry => !entry.IsDirectory))
            {
                // If no files specified, extract all files
                if (ShouldExtractEntry(entry, filesToExtract))
                {
                    entry.WriteToDirectory(outputPath, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                    extractedCount++;
                }
            }

            Logger.Info<ArchiveExtractor>($"Successfully extracted {extractedCount} files from ZIP: {zipPath} to {outputPath}");
        }
        catch (InvalidFormatException ife)
        {
            Logger.Error<ArchiveExtractor>($"Invalid ZIP format for file {zipPath}: {ife.Message}");
            throw;
        }
        catch (UnauthorizedAccessException uae)
        {
            Logger.Error<ArchiveExtractor>($"Access denied when extracting ZIP {zipPath}: {uae.Message}");
            throw;
        }
        catch (IOException ioe)
        {
            Logger.Error<ArchiveExtractor>($"IO error when extracting ZIP {zipPath}: {ioe.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Logger.Error<ArchiveExtractor>($"Unexpected error when extracting ZIP {zipPath}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Extracts all files or specific files from a RAR archive to the specified output directory.
    /// </summary>
    /// <param name="rarPath">The path to the RAR archive to extract.</param>
    /// <param name="outputPath">The directory where files should be extracted.</param>
    /// <param name="filesToExtract">Optional array of specific files to extract. If null or empty, all files are extracted.</param>
    /// <exception cref="ArgumentException">Thrown when rarPath or outputPath is null, empty, or whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified RAR file does not exist.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the output directory does not exist and cannot be created.</exception>
    /// <exception cref="InvalidFormatException">Thrown when the archive format is invalid.</exception>
    private static void ExtractRar(string rarPath, string outputPath, string[]? filesToExtract = null)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(rarPath))
        {
            throw new ArgumentException("RAR path cannot be null or empty.", nameof(rarPath));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path cannot be null or empty.", nameof(outputPath));
        }

        if (!File.Exists(rarPath))
        {
            throw new FileNotFoundException($"RAR file does not exist: {rarPath}");
        }

        try
        {
            Directory.CreateDirectory(outputPath);

            using RarArchive archive = RarArchive.Open(rarPath);
            int extractedCount = 0;

            foreach (RarArchiveEntry entry in archive.Entries.Where(entry => !entry.IsDirectory))
            {
                if (ShouldExtractEntry(entry, filesToExtract))
                {
                    entry.WriteToDirectory(outputPath, new ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                    extractedCount++;
                }
            }

            Logger.Info<ArchiveExtractor>($"Successfully extracted {extractedCount} files from RAR: {rarPath} to {outputPath}");
        }
        catch (InvalidFormatException ife)
        {
            Logger.Error<ArchiveExtractor>($"Invalid RAR format for file {rarPath}: {ife.Message}");
            throw;
        }
        catch (UnauthorizedAccessException uae)
        {
            Logger.Error<ArchiveExtractor>($"Access denied when extracting RAR {rarPath}: {uae.Message}");
            throw;
        }
        catch (IOException ioe)
        {
            Logger.Error<ArchiveExtractor>($"IO error when extracting RAR {rarPath}: {ioe.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Logger.Error<ArchiveExtractor>($"Unexpected error when extracting RAR {rarPath}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Extracts all files or specific files from a TAR archive to the specified output directory.
    /// </summary>
    /// <param name="tarPath">The path to the TAR archive to extract.</param>
    /// <param name="outputPath">The directory where files should be extracted.</param>
    /// <param name="filesToExtract">Optional array of specific files to extract. If null or empty, all files are extracted.</param>
    /// <exception cref="ArgumentException">Thrown when tarPath or outputPath is null, empty, or whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified TAR file does not exist.</exception>
    private static void ExtractTar(string tarPath, string outputPath, string[]? filesToExtract = null)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(tarPath))
        {
            throw new ArgumentException("TAR path cannot be null or empty.", nameof(tarPath));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path cannot be null or empty.", nameof(outputPath));
        }

        if (!File.Exists(tarPath))
        {
            throw new FileNotFoundException($"TAR file does not exist: {tarPath}");
        }

        try
        {
            Directory.CreateDirectory(outputPath);

            using IArchive archive = ArchiveFactory.Open(tarPath);
            int extractedCount = 0;

            foreach (IArchiveEntry entry in archive.Entries.Where(entry => !entry.IsDirectory))
            {
                if (ShouldExtractEntry(entry, filesToExtract))
                {
                    entry.WriteToDirectory(outputPath, new ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                    extractedCount++;
                }
            }

            Logger.Info<ArchiveExtractor>($"Successfully extracted {extractedCount} files from TAR: {tarPath} to {outputPath}");
        }
        catch (InvalidFormatException ife)
        {
            Logger.Error<ArchiveExtractor>($"Invalid TAR format for file {tarPath}: {ife.Message}");
            throw;
        }
        catch (UnauthorizedAccessException uae)
        {
            Logger.Error<ArchiveExtractor>($"Access denied when extracting TAR {tarPath}: {uae.Message}");
            throw;
        }
        catch (IOException ioe)
        {
            Logger.Error<ArchiveExtractor>($"IO error when extracting TAR {tarPath}: {ioe.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Logger.Error<ArchiveExtractor>($"Unexpected error when extracting TAR {tarPath}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Extracts all files or specific files from a 7-Zip archive to the specified output directory.
    /// </summary>
    /// <param name="sevenZipPath">The path to the 7-Zip archive to extract.</param>
    /// <param name="outputPath">The directory where files should be extracted.</param>
    /// <param name="filesToExtract">Optional array of specific files to extract. If null or empty, all files are extracted.</param>
    /// <exception cref="ArgumentException">Thrown when sevenZipPath or outputPath is null, empty, or whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified 7-Zip file does not exist.</exception>
    private static void Extract7Zip(string sevenZipPath, string outputPath, string[]? filesToExtract = null)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(sevenZipPath))
        {
            throw new ArgumentException("7-Zip path cannot be null or empty.", nameof(sevenZipPath));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path cannot be null or empty.", nameof(outputPath));
        }

        if (!File.Exists(sevenZipPath))
        {
            throw new FileNotFoundException($"7-Zip file does not exist: {sevenZipPath}");
        }

        try
        {
            Directory.CreateDirectory(outputPath);

            using IArchive archive = ArchiveFactory.Open(sevenZipPath);
            int extractedCount = 0;

            foreach (IArchiveEntry entry in archive.Entries.Where(entry => !entry.IsDirectory))
            {
                if (ShouldExtractEntry(entry, filesToExtract))
                {
                    entry.WriteToDirectory(outputPath, new ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                    extractedCount++;
                }
            }

            Logger.Info<ArchiveExtractor>($"Successfully extracted {extractedCount} files from 7-Zip: {sevenZipPath} to {outputPath}");
        }
        catch (InvalidFormatException ife)
        {
            Logger.Error<ArchiveExtractor>($"Invalid 7-Zip format for file {sevenZipPath}: {ife.Message}");
            throw;
        }
        catch (UnauthorizedAccessException uae)
        {
            Logger.Error<ArchiveExtractor>($"Access denied when extracting 7-Zip {sevenZipPath}: {uae.Message}");
            throw;
        }
        catch (IOException ioe)
        {
            Logger.Error<ArchiveExtractor>($"IO error when extracting 7-Zip {sevenZipPath}: {ioe.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Logger.Error<ArchiveExtractor>($"Unexpected error when extracting 7-Zip {sevenZipPath}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Extracts all files or specific files from an archive to the specified output directory.
    /// Automatically detects the archive format based on the file extension.
    /// </summary>
    /// <param name="archivePath">The path to the archive to extract.</param>
    /// <param name="outputPath">The directory where files should be extracted.</param>
    /// <param name="filesToExtract">Optional array of specific files to extract. If null or empty, all files are extracted.</param>
    /// <exception cref="ArgumentException">Thrown when archivePath or outputPath is null, empty, or whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified archive file does not exist.</exception>
    /// <exception cref="NotSupportedException">Thrown when the archive format is not supported.</exception>
    public static void ExtractArchive(string archivePath, string outputPath, string[]? filesToExtract = null)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(archivePath))
        {
            throw new ArgumentException("Archive path cannot be null or empty.", nameof(archivePath));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path cannot be null or empty.", nameof(outputPath));
        }

        if (!File.Exists(archivePath))
        {
            throw new FileNotFoundException($"Archive file does not exist: {archivePath}");
        }

        string extension = Path.GetExtension(archivePath).ToLowerInvariant();
        switch (extension)
        {
            case ".zip":
                ExtractZip(archivePath, outputPath, filesToExtract);
                break;
            case ".rar":
                ExtractRar(archivePath, outputPath, filesToExtract);
                break;
            case ".tar":
            case ".tar.gz":
            case ".tgz":
            case ".tar.bz2":
            case ".tbz2":
            case ".tar.xz":
            case ".txz":
                ExtractTar(archivePath, outputPath, filesToExtract);
                break;
            case ".7z":
                Extract7Zip(archivePath, outputPath, filesToExtract);
                break;
            default:
                string supportedFormats = ".zip, .rar, .tar, .tar.gz, .tgz, .tar.bz2, .tbz2, .tar.xz, .txz, .7z";
                throw new NotSupportedException($"Archive format '{extension}' is not supported. Supported formats: {supportedFormats}");
        }
    }

    /// <summary>
    /// Extracts all files or specific files from a ZIP archive to the specified output directory asynchronously.
    /// </summary>
    /// <param name="zipPath">The path to the ZIP archive to extract.</param>
    /// <param name="outputPath">The directory where files should be extracted.</param>
    /// <param name="filesToExtract">Optional array of specific files to extract. If null or empty, all files are extracted.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the extraction operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when zipPath or outputPath is null, empty, or whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified ZIP file does not exist.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the output directory does not exist and cannot be created.</exception>
    /// <exception cref="InvalidFormatException">Thrown when the archive format is invalid.</exception>
    private static async Task ExtractZipAsync(string zipPath, string outputPath, string[]? filesToExtract = null, CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(zipPath))
        {
            throw new ArgumentException("ZIP path cannot be null or empty.", nameof(zipPath));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path cannot be null or empty.", nameof(outputPath));
        }

        if (!File.Exists(zipPath))
        {
            throw new FileNotFoundException($"ZIP file does not exist: {zipPath}");
        }

        try
        {
            Directory.CreateDirectory(outputPath);

            using IArchive archive = ArchiveFactory.Open(zipPath);
            int extractedCount = 0;

            foreach (IArchiveEntry entry in archive.Entries.Where(entry => !entry.IsDirectory))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (ShouldExtractEntry(entry, filesToExtract))
                {
                    await Task.Run(() => entry.WriteToDirectory(outputPath, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    }), cancellationToken);
                    extractedCount++;
                }
            }

            Logger.Info<ArchiveExtractor>($"Successfully extracted {extractedCount} files from ZIP: {zipPath} to {outputPath}");
        }
        catch (OperationCanceledException)
        {
            Logger.Warning<ArchiveExtractor>($"Extraction was cancelled for ZIP: {zipPath}");
            throw;
        }
        catch (InvalidFormatException ife)
        {
            Logger.Error<ArchiveExtractor>($"Invalid ZIP format for file {zipPath}: {ife.Message}");
            throw;
        }
        catch (UnauthorizedAccessException uae)
        {
            Logger.Error<ArchiveExtractor>($"Access denied when extracting ZIP {zipPath}: {uae.Message}");
            throw;
        }
        catch (IOException ioe)
        {
            Logger.Error<ArchiveExtractor>($"IO error when extracting ZIP {zipPath}: {ioe.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Logger.Error<ArchiveExtractor>($"Unexpected error when extracting ZIP {zipPath}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Extracts all files or specific files from a RAR archive to the specified output directory asynchronously.
    /// </summary>
    /// <param name="rarPath">The path to the RAR archive to extract.</param>
    /// <param name="outputPath">The directory where files should be extracted.</param>
    /// <param name="filesToExtract">Optional array of specific files to extract. If null or empty, all files are extracted.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the extraction operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when rarPath or outputPath is null, empty, or whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified RAR file does not exist.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the output directory does not exist and cannot be created.</exception>
    /// <exception cref="InvalidFormatException">Thrown when the archive format is invalid.</exception>
    private static async Task ExtractRarAsync(string rarPath, string outputPath, string[]? filesToExtract = null, CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(rarPath))
        {
            throw new ArgumentException("RAR path cannot be null or empty.", nameof(rarPath));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path cannot be null or empty.", nameof(outputPath));
        }

        if (!File.Exists(rarPath))
        {
            throw new FileNotFoundException($"RAR file does not exist: {rarPath}");
        }

        try
        {
            Directory.CreateDirectory(outputPath);

            using RarArchive archive = RarArchive.Open(rarPath);
            int extractedCount = 0;

            foreach (RarArchiveEntry entry in archive.Entries.Where(entry => !entry.IsDirectory))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (ShouldExtractEntry(entry, filesToExtract))
                {
                    await Task.Run(() => entry.WriteToDirectory(outputPath, new ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    }), cancellationToken);
                    extractedCount++;
                }
            }

            Logger.Info<ArchiveExtractor>($"Successfully extracted {extractedCount} files from RAR: {rarPath} to {outputPath}");
        }
        catch (OperationCanceledException)
        {
            Logger.Warning<ArchiveExtractor>($"Extraction was cancelled for RAR: {rarPath}");
            throw;
        }
        catch (InvalidFormatException ife)
        {
            Logger.Error<ArchiveExtractor>($"Invalid RAR format for file {rarPath}: {ife.Message}");
            throw;
        }
        catch (UnauthorizedAccessException uae)
        {
            Logger.Error<ArchiveExtractor>($"Access denied when extracting RAR {rarPath}: {uae.Message}");
            throw;
        }
        catch (IOException ioe)
        {
            Logger.Error<ArchiveExtractor>($"IO error when extracting RAR {rarPath}: {ioe.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Logger.Error<ArchiveExtractor>($"Unexpected error when extracting RAR {rarPath}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Extracts all files or specific files from a TAR archive to the specified output directory asynchronously.
    /// </summary>
    /// <param name="tarPath">The path to the TAR archive to extract.</param>
    /// <param name="outputPath">The directory where files should be extracted.</param>
    /// <param name="filesToExtract">Optional array of specific files to extract. If null or empty, all files are extracted.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the extraction operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when tarPath or outputPath is null, empty, or whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified TAR file does not exist.</exception>
    private static async Task ExtractTarAsync(string tarPath, string outputPath, string[]? filesToExtract = null, CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(tarPath))
        {
            throw new ArgumentException("TAR path cannot be null or empty.", nameof(tarPath));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path cannot be null or empty.", nameof(outputPath));
        }

        if (!File.Exists(tarPath))
        {
            throw new FileNotFoundException($"TAR file does not exist: {tarPath}");
        }

        try
        {
            Directory.CreateDirectory(outputPath);

            using IArchive archive = ArchiveFactory.Open(tarPath);
            int extractedCount = 0;

            foreach (IArchiveEntry entry in archive.Entries.Where(entry => !entry.IsDirectory))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (ShouldExtractEntry(entry, filesToExtract))
                {
                    await Task.Run(() => entry.WriteToDirectory(outputPath, new ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    }), cancellationToken);
                    extractedCount++;
                }
            }

            Logger.Info<ArchiveExtractor>($"Successfully extracted {extractedCount} files from TAR: {tarPath} to {outputPath}");
        }
        catch (OperationCanceledException)
        {
            Logger.Warning<ArchiveExtractor>($"Extraction was cancelled for TAR: {tarPath}");
            throw;
        }
        catch (InvalidFormatException ife)
        {
            Logger.Error<ArchiveExtractor>($"Invalid TAR format for file {tarPath}: {ife.Message}");
            throw;
        }
        catch (UnauthorizedAccessException uae)
        {
            Logger.Error<ArchiveExtractor>($"Access denied when extracting TAR {tarPath}: {uae.Message}");
            throw;
        }
        catch (IOException ioe)
        {
            Logger.Error<ArchiveExtractor>($"IO error when extracting TAR {tarPath}: {ioe.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Logger.Error<ArchiveExtractor>($"Unexpected error when extracting TAR {tarPath}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Extracts all files or specific files from a 7-Zip archive to the specified output directory asynchronously.
    /// </summary>
    /// <param name="sevenZipPath">The path to the 7-Zip archive to extract.</param>
    /// <param name="outputPath">The directory where files should be extracted.</param>
    /// <param name="filesToExtract">Optional array of specific files to extract. If null or empty, all files are extracted.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the extraction operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when sevenZipPath or outputPath is null, empty, or whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified 7-Zip file does not exist.</exception>
    private static async Task Extract7ZipAsync(string sevenZipPath, string outputPath, string[]? filesToExtract = null, CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(sevenZipPath))
        {
            throw new ArgumentException("7-Zip path cannot be null or empty.", nameof(sevenZipPath));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path cannot be null or empty.", nameof(outputPath));
        }

        if (!File.Exists(sevenZipPath))
        {
            throw new FileNotFoundException($"7-Zip file does not exist: {sevenZipPath}");
        }

        try
        {
            Directory.CreateDirectory(outputPath);

            using IArchive archive = ArchiveFactory.Open(sevenZipPath);
            int extractedCount = 0;

            foreach (IArchiveEntry entry in archive.Entries.Where(entry => !entry.IsDirectory))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (ShouldExtractEntry(entry, filesToExtract))
                {
                    await Task.Run(() => entry.WriteToDirectory(outputPath, new ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    }), cancellationToken);
                    extractedCount++;
                }
            }

            Logger.Info<ArchiveExtractor>($"Successfully extracted {extractedCount} files from 7-Zip: {sevenZipPath} to {outputPath}");
        }
        catch (OperationCanceledException)
        {
            Logger.Warning<ArchiveExtractor>($"Extraction was cancelled for 7-Zip: {sevenZipPath}");
            throw;
        }
        catch (InvalidFormatException ife)
        {
            Logger.Error<ArchiveExtractor>($"Invalid 7-Zip format for file {sevenZipPath}: {ife.Message}");
            throw;
        }
        catch (UnauthorizedAccessException uae)
        {
            Logger.Error<ArchiveExtractor>($"Access denied when extracting 7-Zip {sevenZipPath}: {uae.Message}");
            throw;
        }
        catch (IOException ioe)
        {
            Logger.Error<ArchiveExtractor>($"IO error when extracting 7-Zip {sevenZipPath}: {ioe.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Logger.Error<ArchiveExtractor>($"Unexpected error when extracting 7-Zip {sevenZipPath}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Extracts all files or specific files from an archive to the specified output directory asynchronously.
    /// Automatically detects the archive format based on the file extension.
    /// </summary>
    /// <param name="archivePath">The path to the archive to extract.</param>
    /// <param name="outputPath">The directory where files should be extracted.</param>
    /// <param name="filesToExtract">Optional array of specific files to extract. If null or empty, all files are extracted.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the extraction operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when archivePath or outputPath is null, empty, or whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified archive file does not exist.</exception>
    /// <exception cref="NotSupportedException">Thrown when the archive format is not supported.</exception>
    public static async Task ExtractArchiveAsync(string archivePath, string outputPath, string[]? filesToExtract = null, CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(archivePath))
        {
            throw new ArgumentException("Archive path cannot be null or empty.", nameof(archivePath));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path cannot be null or empty.", nameof(outputPath));
        }

        if (!File.Exists(archivePath))
        {
            throw new FileNotFoundException($"Archive file does not exist: {archivePath}");
        }

        string extension = Path.GetExtension(archivePath).ToLowerInvariant();
        switch (extension)
        {
            case ".zip":
                await ExtractZipAsync(archivePath, outputPath, filesToExtract, cancellationToken);
                break;
            case ".rar":
                await ExtractRarAsync(archivePath, outputPath, filesToExtract, cancellationToken);
                break;
            case ".tar":
            case ".tar.gz":
            case ".tgz":
            case ".tar.bz2":
            case ".tbz2":
            case ".tar.xz":
            case ".txz":
                await ExtractTarAsync(archivePath, outputPath, filesToExtract, cancellationToken);
                break;
            case ".7z":
                await Extract7ZipAsync(archivePath, outputPath, filesToExtract, cancellationToken);
                break;
            default:
                string supportedFormats = ".zip, .rar, .tar, .tar.gz, .tgz, .tar.bz2, .tbz2, .tar.xz, .txz, .7z";
                throw new NotSupportedException($"Archive format '{extension}' is not supported. Supported formats: {supportedFormats}");
        }
    }

    /// <summary>
    /// Determines whether an archive entry should be extracted based on the specified file filters.
    /// </summary>
    /// <param name="entry">The archive entry to check.</param>
    /// <param name="filesToExtract">Array of specific files to extract. If null or empty, all files are extracted.</param>
    /// <returns>True if the entry should be extracted, false otherwise.</returns>
    private static bool ShouldExtractEntry(IArchiveEntry entry, string[]? filesToExtract)
    {
        // If no files specified, extract all files
        if (filesToExtract == null || filesToExtract.Length == 0)
        {
            return true;
        }

        // Check if this entry matches any of the specified files
        return !string.IsNullOrEmpty(entry.Key) &&
               filesToExtract.Any(fileToExtract =>
                   entry.Key.Equals(fileToExtract, StringComparison.OrdinalIgnoreCase) ||
                   entry.Key.EndsWith(fileToExtract, StringComparison.OrdinalIgnoreCase));
    }
}