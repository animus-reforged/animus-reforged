using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

namespace AnimusReforged.Mods.Utilities;

public static class Extractor
{
    public static void ExtractZip(string zipPath, string outputPath, string[]? filesToExtract = null)
    {
        using IArchive archive = ArchiveFactory.Open(zipPath);
        foreach (IArchiveEntry entry in archive.Entries.Where(entry => !entry.IsDirectory))
        {
            // If no files specified, extract all files
            if (filesToExtract == null || filesToExtract.Length == 0)
            {
                entry.WriteToDirectory(outputPath, new ExtractionOptions()
                {
                    ExtractFullPath = true,
                    Overwrite = true
                });
                continue;
            }

            // Check if this entry matches any of the specified files
            bool shouldExtract = !string.IsNullOrEmpty(entry.Key) &&
                                 filesToExtract.Any(fileToExtract => entry.Key.Equals(fileToExtract, StringComparison.OrdinalIgnoreCase) || entry.Key.EndsWith(fileToExtract, StringComparison.OrdinalIgnoreCase));

            if (shouldExtract)
            {
                entry.WriteToDirectory(outputPath, new ExtractionOptions()
                {
                    ExtractFullPath = true,
                    Overwrite = true
                });
            }
        }
    }

    public static void ExtractRar(string rarPath, string outputPath, string[]? filesToExtract = null)
    {
        using RarArchive archive = RarArchive.Open(rarPath);
        foreach (RarArchiveEntry entry in archive.Entries.Where(entry => !entry.IsDirectory))
        {
            if (filesToExtract == null || filesToExtract.Length == 0)
            {
                entry.WriteToDirectory(outputPath, new ExtractionOptions
                {
                    ExtractFullPath = true,
                    Overwrite = true
                });
                continue;
            }

            // Check if this entry matches any of the specified files
            bool shouldExtract = !string.IsNullOrEmpty(entry.Key) &&
                                 filesToExtract.Any(fileToExtract => entry.Key.Equals(fileToExtract, StringComparison.OrdinalIgnoreCase) || entry.Key.EndsWith(fileToExtract, StringComparison.OrdinalIgnoreCase));

            if (shouldExtract)
            {
                entry.WriteToDirectory(outputPath, new ExtractionOptions
                {
                    ExtractFullPath = true,
                    Overwrite = true
                });
            }
        }
    }
}