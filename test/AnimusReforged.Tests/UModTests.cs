using System.Text;
using AnimusReforged.Mods.Utilities;
using AnimusReforged.Paths;

namespace AnimusReforged.Tests;

public class UModTests
{
    [SetUp]
    public void Setup()
    {
        if (Directory.Exists(AppPaths.uModAppdata))
        {
            Directory.Delete(AppPaths.uModAppdata, true);
        }
    }

    [Test]
    public void AppdataSetupTest()
    {
        // Testing fresh
        string gamePath1 = @"E:\Games\Assassins Creed (Steam)\AssassinsCreed_Dx9.exe";
        string gamePath2 = @"E:\Games\Assassins Creed (Steam)\AssassinsCreed_Dx9 (Patched Windowed Mode).exe";
        UModManager.SetupAppdata(gamePath1).Wait();
        
        // Testing with existing AppData
        UModManager.SetupAppdata(gamePath2).Wait();
        Assert.Pass();
    }
    
    [Test]
    public void SaveFileSetupTest()
    {
        // Testing fresh
        string saveFileEntry = $"{@"E:\Games\Assassins Creed (Steam)\AssassinsCreed_Dx9.exe"}|{"ac1.txt"}\n";
        File.AppendAllTextAsync("test1.txt", saveFileEntry, new UnicodeEncoding(false, false)).Wait();
        byte[] bytes = Encoding.Unicode.GetBytes(saveFileEntry); // UTF-16LE, no BOM
        using (FileStream stream = new FileStream("test2.txt", FileMode.Append, FileAccess.Write))
        {
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}