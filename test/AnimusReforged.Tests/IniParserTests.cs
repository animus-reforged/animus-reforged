using System.Text;
using AnimusReforged.Mods.Utilities;
using AnimusReforged.Paths;

namespace AnimusReforged.Tests;

public class IniParserTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestIniParser()
    {
        IniParser parsedIni = new IniParser("EaglePatchAC1.ini");
        int keyboardLayout = parsedIni.GetInt("EaglePatchAC1", "KeyboardLayout");
        if (keyboardLayout == 0)
        {
            Assert.Pass();
        }
        else
        {
            Assert.Fail();
        }
    }

    [Test]
    public void TestIniSave()
    {
        IniParser parsedIni = new IniParser("EaglePatchAC1.ini");
        parsedIni.Set("EaglePatchAC1", "SkipIntroVideos", 1);
        parsedIni.Save();
    }
}