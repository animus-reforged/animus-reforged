using AnimusReforged.Mods.Utilities;

namespace AnimusReforged.Tests;

public class PatcherTests
{
    [SetUp]
    public void Setup()
    {
        Logger.Initialize();
    }

    [Test]
    public void LargeAwareAddressPatch()
    {
        // Testing fresh
        string gamePath = @"AssassinsCreed_Dx9.exe";
        try
        {
            UniversalPatcher.LargeAddressAwarePatch(gamePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Assert.Fail();
        }
        Assert.Pass();
    }
}