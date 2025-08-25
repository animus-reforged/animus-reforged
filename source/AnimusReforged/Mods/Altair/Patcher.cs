using System.Text;

namespace AnimusReforged.Mods.Altair;

public class Patcher
{
    // Variables
    private const string UBISOFT_SERVER_URL = "gconnect.ubi.com";

    // Methods
    public static void StutterPatch(string gamePath)
    {
        if (!File.Exists(gamePath))
        {
            throw new FileNotFoundException("Game file not found");
        }
        Logger.Debug("Applying StutterPatch");
        Logger.Debug("Reading file bytes");
        byte[] fileBytes = File.ReadAllBytes(gamePath);
        Logger.Debug($"File size: {fileBytes.Length} bytes");
        byte[] targetBytes = Encoding.ASCII.GetBytes(UBISOFT_SERVER_URL);
        Logger.Debug($"Target string: '{UBISOFT_SERVER_URL}' ({targetBytes.Length} bytes)");
        Logger.Debug($"Target bytes: {BitConverter.ToString(targetBytes)}");

        Logger.Debug("Searching for target sequence");
        int position = FindSequence(fileBytes, targetBytes);
        if (position == -1)
        {
            throw new Exception("Failed to find target string in the file.");
        }

        Logger.Info($"Found {UBISOFT_SERVER_URL} at offset 0x{position:X}");
        Logger.Debug($"Original byte at position {position}: 0x{fileBytes[position]:X2}");
        
        // Replacing the first byte of the sequence with 0x00
        fileBytes[position] = 0x00;
        Logger.Debug($"Replaced byte at position {position} with 0x00");
        
        Logger.Debug("Writing patched file...");
        File.WriteAllBytes(gamePath, fileBytes);
        Logger.Info("StutterPatch applied successfully");
    }

    public static void StutterPatchRevert(string gamePath)
    {
        if (!File.Exists(gamePath))
        {
            Logger.Debug($"File not found: {gamePath}");
            throw new FileNotFoundException("Game file not found");
        }

        Logger.Debug("Reverting StutterPatch");
        Logger.Debug("Reading file bytes");
        byte[] fileBytes = File.ReadAllBytes(gamePath);
        Logger.Debug($"File size: {fileBytes.Length} bytes");
        
        byte[] originalBytes = Encoding.ASCII.GetBytes(UBISOFT_SERVER_URL);
        Logger.Debug($"Original string: '{UBISOFT_SERVER_URL}' ({originalBytes.Length} bytes)");
        
        // Look for the patched pattern: 0x00 + "connect.ubi.com" (rest of original string)
        string remainingUrl = UBISOFT_SERVER_URL.Substring(1); // "connect.ubi.com"
        byte[] remainingBytes = Encoding.ASCII.GetBytes(remainingUrl);
        
        Logger.Debug($"Looking for remaining URL after patch: '{remainingUrl}'");
        Logger.Debug($"Remaining bytes: {BitConverter.ToString(remainingBytes)}");
        Logger.Debug("Searching for remaining URL sequence");
        
        // Find the position of "connect.ubi.com"
        int remainingPosition = FindSequence(fileBytes, remainingBytes);
        if (remainingPosition == -1)
        {
            Logger.Debug("Remaining URL sequence not found - file may not be patched or already reverted");
            throw new Exception("Failed to find patched sequence in the file. File may not be patched.");
        }

        // The null byte should be one position before "connect.ubi.com"
        int patchPosition = remainingPosition - 1;
        
        if (patchPosition < 0 || fileBytes[patchPosition] != 0x00)
        {
            Logger.Debug($"Expected null byte at position {patchPosition}, but found 0x{(patchPosition >= 0 ? fileBytes[patchPosition] : 0):X2}");
            throw new Exception("File doesn't appear to be properly patched - null byte not found at expected position.");
        }

        Logger.Info($"Found patched sequence: null byte at offset 0x{patchPosition:X}, remaining URL at 0x{remainingPosition:X}");
        Logger.Debug($"Current byte at position {patchPosition}: 0x{fileBytes[patchPosition]:X2}");
        
        // Restore the original first byte 'g'
        fileBytes[patchPosition] = originalBytes[0]; // Restore 'g' from "gconnect.ubi.com"
        Logger.Debug($"Restored byte at position {patchPosition} to 0x{fileBytes[patchPosition]:X2} ('{(char)fileBytes[patchPosition]}')");
        
        // Verify the full original string is now restored
        string restoredUrl = Encoding.ASCII.GetString(fileBytes, patchPosition, originalBytes.Length);
        Logger.Debug($"Restored URL: '{restoredUrl}'");
        
        Logger.Debug("Writing reverted file...");
        File.WriteAllBytes(gamePath, fileBytes);
        Logger.Info("Reverting StutterPatch completed successfully");
    }

    private static int FindSequence(byte[] fileBytes, byte[] targetBytes)
    {
        Logger.Debug($"Searching for sequence of {targetBytes.Length} bytes in {fileBytes.Length} byte file");
        for (int i = 0; i < fileBytes.Length - targetBytes.Length; i++)
        {
            // Logger.Debug($"Checking position {i} (0x{i:X})");
            bool match = true;
            for (int j = 0; j < targetBytes.Length; j++)
            {
                if (fileBytes[i + j] != targetBytes[j])
                {
                    // Logger.Debug($"Mismatch at position {i + j}: expected 0x{targetBytes[j]:X2}, found 0x{fileBytes[i + j]:X2}");
                    match = false;
                    break;
                }
            }
            if (match)
            {
                Logger.Debug($"Found complete match at position {i}");
                return i;
            }
        }
        Logger.Debug("No match found");
        return -1;
    }
}