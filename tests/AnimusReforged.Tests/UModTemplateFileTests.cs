using AnimusReforged.Mods.Core;

namespace AnimusReforged.Tests;

[TestFixture]
public class UModTemplateFileTests
{
    private const string TemplateTestFile = "Assets/TemplateTest.txt";
    private const string TempTemplateFile = "TempTemplateTest.txt";

    [SetUp]
    public void Setup()
    {
        // Ensure the test file exists in the output directory
        if (!File.Exists(TemplateTestFile))
        {
            Assert.Fail($"Template test file '{TemplateTestFile}' does not exist in output directory.");
        }
    }

    [Test]
    public void Constructor_WithValidTemplateFile_ParsesTemplateSuccessfully()
    {
        // Arrange & Act
        UModTemplateFile templateFile = new UModTemplateFile(TemplateTestFile, "Mods");

        // Assert
        Assert.That(templateFile, Is.Not.Null);
    }

    [Test]
    public void Constructor_WithNonExistentTemplateFile_ThrowsFileNotFoundException()
    {
        // Arrange
        const string nonExistentFile = "NonExistentTemplate.txt";

        // Act & Assert
        FileNotFoundException exception = Assert.Throws<FileNotFoundException>(() =>
        {
            UModTemplateFile uModTemplateFile = new UModTemplateFile(nonExistentFile, "Mods");
        });

        Assert.That(exception.Message, Does.Contain("Template file not found"));
    }

    [Test]
    public void Constructor_WithNullTemplatePath_ThrowsArgumentNullException()
    {
        // Act & Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
        {
            UModTemplateFile uModTemplateFile = new UModTemplateFile(null!, "Mods");
        });

        Assert.That(exception.ParamName, Is.EqualTo("templatePath"));
    }

    [Test]
    public void Constructor_WithNullModsPath_ThrowsArgumentNullException()
    {
        // Act & Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
        {
            UModTemplateFile uModTemplateFile = new UModTemplateFile(TemplateTestFile, null!);
        });

        Assert.That(exception.ParamName, Is.EqualTo("modsPath"));
    }

    [Test]
    public void ParseTemplate_WithValidFile_ParsesHeaderAndEnabledModsCorrectly()
    {
        // Arrange - Create a temporary mods directory for testing
        string testModsDir = "TestModsForParse";
        Directory.CreateDirectory(testModsDir);
        File.WriteAllText(Path.Combine(testModsDir, "TestMod.tpf"), "dummy content");

        try
        {
            UModTemplateFile templateFile = new UModTemplateFile(TemplateTestFile, testModsDir);

            // The TemplateTest.txt contains:
            // SaveAllTextures:0
            // SaveSingleTexture:0
            // FontColour:255,0,0
            // TextureColour:0,255,0
            // Add_true:E:\Overhaul\Overhaul.tpf

            // Act & Assert - Load the mods to verify parsing worked correctly
            (IReadOnlyList<string> enabledMods, IReadOnlyList<string> disabledMods) = templateFile.LoadMods();

            // The enabled mod from the template file should be present
            Assert.That(enabledMods, Contains.Item(@"E:\Overhaul\Overhaul.tpf"));
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(testModsDir))
            {
                Directory.Delete(testModsDir, true);
            }
        }
    }

    [Test]
    public void LoadMods_WithNonExistentModsDirectory_ThrowsDirectoryNotFoundException()
    {
        // Arrange
        UModTemplateFile templateFile = new UModTemplateFile(TemplateTestFile, "NonExistentModsDir");

        // Act & Assert
        DirectoryNotFoundException exception = Assert.Throws<DirectoryNotFoundException>(() => templateFile.LoadMods());

        Assert.That(exception.Message, Does.Contain("Mods directory not found"));
    }

    [Test]
    public void LoadModsAsync_WithNonExistentModsDirectory_ThrowsDirectoryNotFoundException()
    {
        // Arrange
        UModTemplateFile templateFile = new UModTemplateFile(TemplateTestFile, "NonExistentModsDir");

        // Act & Assert
        DirectoryNotFoundException exception = Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await templateFile.LoadModsAsync());

        Assert.That(exception.Message, Does.Contain("Mods directory not found"));
    }

    [Test]
    public void SaveEnabledMods_WithValidMods_SavesTemplateFileCorrectly()
    {
        // Arrange - Copy the template file to a temporary file for testing
        File.Copy(TemplateTestFile, TempTemplateFile, true);
        UModTemplateFile templateFile = new UModTemplateFile(TempTemplateFile, "Mods");

        List<string> newEnabledMods = [@"C:\Mods\TestMod1.tpf", @"D:\AnotherMod.tpf"];

        // Act
        templateFile.SaveEnabledMods(newEnabledMods);

        // Assert - Check that the file was updated correctly
        string[] lines = File.ReadAllLines(TempTemplateFile);

        // Check that the header lines are preserved
        Assert.That(lines[0], Is.EqualTo("SaveAllTextures:0"));
        Assert.That(lines[1], Is.EqualTo("SaveSingleTexture:0"));
        Assert.That(lines[2], Is.EqualTo("FontColour:255,0,0"));
        Assert.That(lines[3], Is.EqualTo("TextureColour:0,255,0"));

        // Check that the new enabled mods are added
        Assert.That(lines[4], Is.EqualTo("Add_true:C:\\Mods\\TestMod1.tpf"));
        Assert.That(lines[5], Is.EqualTo("Add_true:D:\\AnotherMod.tpf"));

        // Cleanup
        if (File.Exists(TempTemplateFile))
        {
            File.Delete(TempTemplateFile);
        }
    }

    [Test]
    public void SaveEnabledMods_WithNullEnabledMods_ThrowsArgumentNullException()
    {
        // Arrange
        UModTemplateFile templateFile = new UModTemplateFile(TemplateTestFile, "Mods");

        // Act & Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => templateFile.SaveEnabledMods(null!));

        Assert.That(exception.ParamName, Is.EqualTo("enabledMods"));
    }

    [Test]
    public void Refresh_ReparsesTemplateFileAfterChanges()
    {
        // Arrange - Create a temporary mods directory for testing
        string testModsDir = "TestModsForRefresh";
        Directory.CreateDirectory(testModsDir);
        File.WriteAllText(Path.Combine(testModsDir, "TestMod.tpf"), "dummy content");

        try
        {
            // Copy the template file to a temporary file for testing
            File.Copy(TemplateTestFile, TempTemplateFile, true);
            UModTemplateFile templateFile = new UModTemplateFile(TempTemplateFile, testModsDir);

            // Initially, the template has one enabled mod
            (IReadOnlyList<string> initialEnabledMods, _) = templateFile.LoadMods();
            Assert.That(initialEnabledMods, Contains.Item(@"E:\Overhaul\Overhaul.tpf"));

            // Modify the template file
            File.WriteAllText(TempTemplateFile,
                "NewHeader:Value\nAdd_true:C:\\NewMod.tpf\nAdd_true:D:\\AnotherNewMod.tpf");

            // Act - Refresh the template file
            templateFile.Refresh();

            // Assert - The new enabled mods should be loaded
            (IReadOnlyList<string> updatedEnabledMods, _) = templateFile.LoadMods();
            Assert.That(updatedEnabledMods, Contains.Item(@"C:\NewMod.tpf"));
            Assert.That(updatedEnabledMods, Contains.Item(@"D:\AnotherNewMod.tpf"));
            Assert.That(updatedEnabledMods.Count, Is.EqualTo(2));
        }
        finally
        {
            // Cleanup
            if (File.Exists(TempTemplateFile))
            {
                File.Delete(TempTemplateFile);
            }
            if (Directory.Exists(testModsDir))
            {
                Directory.Delete(testModsDir, true);
            }
        }
    }
}