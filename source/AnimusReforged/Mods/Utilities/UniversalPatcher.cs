using System.Text;

namespace AnimusReforged.Mods.Utilities;

public class UniversalPatcher
{
    // Variables
    // 4GB Patch Constants
    private const int PE_POINTER_OFFSET = 0x3C;
    private const ushort IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x20;
    
    // Methods
    public static void LargeAddressAwarePatch(string gamePath)
    {
        if (!File.Exists(gamePath))
        {
            Logger.Error("Game executable does not exist");
            throw new FileNotFoundException("Game executable does not exist");
        }

        Logger.Debug("Applying 4GB Patch");
        using FileStream stream = new FileStream(gamePath, FileMode.Open, FileAccess.ReadWrite);
        using BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, true);
        using BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, true);
        
        // Locate PE Header
        stream.Seek(PE_POINTER_OFFSET, SeekOrigin.Begin);
        int peHeaderAddress = reader.ReadInt32();
        Logger.Debug($"PE Header Address: 0x{peHeaderAddress:X}");

        stream.Seek(peHeaderAddress, SeekOrigin.Begin);
        uint peSignature = reader.ReadUInt32(); // "PE\0\0"
        Logger.Debug($"PE Signature: 0x{peSignature:X}");

        if (peSignature != 0x4550)
        {
            throw new Exception("Not a valid PE file (Invalid PE Signature)");
        }

        Logger.Debug("Valid PE file detected");

        // Navigate to File Header Characteristics
        stream.Seek(4, SeekOrigin.Current); // Skip Machine (2) + NumberOfSections (2) = 4
        stream.Seek(12, SeekOrigin.Current); // TimeDateStamp (4) + PointerToSymbolTable (4) + NumberOfSymbols (4) = 12
        stream.Seek(2, SeekOrigin.Current); // SizeOfOptionalHeader = 2

        long characteristicsPosition = stream.Position;
        ushort characteristics = reader.ReadUInt16();

        Logger.Debug($"Current FileHeader Characteristics: 0x{characteristics:X4}");
        if ((characteristics & IMAGE_FILE_LARGE_ADDRESS_AWARE) == 0)
        {
            characteristics |= IMAGE_FILE_LARGE_ADDRESS_AWARE;
            stream.Seek(characteristicsPosition, SeekOrigin.Begin);
            writer.Write(characteristics);
            Logger.Debug($"Updated FileHeader Characteristics: 0x{characteristics:X4}");
            Logger.Info("Large Address Aware applied successfully");
        }
        else
        {
            Logger.Warning("File is already Large Address Aware - no patch needed");
        }
    }
}