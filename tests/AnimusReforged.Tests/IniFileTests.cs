using AnimusReforged.Utilities.Ini;

namespace AnimusReforged.Tests;

[TestFixture]
public class IniFileTests
{
    private const string TestIniFile = "Assets/IniFileTest.ini";
    private const string TempIniFile = "TempTest.ini";
    
    [SetUp]
    public void Setup()
    {
        // Ensure the test file exists in the output directory
        if (!File.Exists(TestIniFile))
        {
            Assert.Fail($"Test INI file '{TestIniFile}' does not exist in output directory.");
        }
    }

    [Test]
    public void Constructor_WithValidFile_LoadsIniDataSuccessfully()
    {
        // Arrange & Act
        IniFile iniFile = new IniFile(TestIniFile);

        // Assert
        Assert.That(iniFile, Is.Not.Null);
    }

    [Test]
    public void Constructor_WithNonExistentFile_ThrowsIOException()
    {
        // Arrange
        const string nonExistentFile = "NonExistent.ini";

        // Act & Assert
        IOException? exception = Assert.Throws<IOException>(() => new IniFile(nonExistentFile));
        Assert.That(exception.Message, Does.Contain("Couldn't find the selected INI file"));
    }

    [Test]
    public void Constructor_WithNullPath_ThrowsArgumentException()
    {
        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => new IniFile(null!));
        Assert.That(exception.Message, Does.Contain("INI file path cannot be null or empty"));
    }

    [Test]
    public void Constructor_WithEmptyPath_ThrowsArgumentException()
    {
        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => new IniFile(""));
        Assert.That(exception.Message, Does.Contain("INI file path cannot be null or empty"));
    }

    [Test]
    public void Get_WithValidSectionAndKey_ReturnsCorrectValue()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        string version = iniFile.Get("Metadata", "Version");
        string windowMode = iniFile.Get("Display", "WindowMode");

        // Assert
        Assert.That(version, Is.EqualTo("0.2.1"));
        Assert.That(windowMode, Is.EqualTo("1"));
    }

    [Test]
    public void Get_WithNonExistentKey_ReturnsDefaultValue()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        string result = iniFile.Get("Display", "NonExistentKey", "default");

        // Assert
        Assert.That(result, Is.EqualTo("default"));
    }

    [Test]
    public void Get_WithNonExistentSection_ReturnsDefaultValue()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        string result = iniFile.Get("NonExistentSection", "AnyKey", "default");

        // Assert
        Assert.That(result, Is.EqualTo("default"));
    }

    [Test]
    public void GetInt_WithValidIntegerValue_ReturnsCorrectValue()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        int windowMode = iniFile.GetInt("Display", "WindowMode");
        int windowWidth = iniFile.GetInt("Display", "WindowWidth");
        int windowHeight = iniFile.GetInt("Display", "WindowHeight");

        // Assert
        Assert.That(windowMode, Is.EqualTo(1));
        Assert.That(windowWidth, Is.EqualTo(1280));
        Assert.That(windowHeight, Is.EqualTo(720));
    }

    [Test]
    public void GetInt_WithNonIntegerValue_ReturnsDefaultValue()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        int result = iniFile.GetInt("Metadata", "Version", 999);

        // Assert
        Assert.That(result, Is.EqualTo(999));
    }

    [Test]
    public void GetInt_WithNonExistentKey_ReturnsDefaultValue()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        int result = iniFile.GetInt("Display", "NonExistentKey", 42);

        // Assert
        Assert.That(result, Is.EqualTo(42));
    }

    [Test]
    public void GetBool_WithValidIntegerValue_ReturnsCorrectValue()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        bool highCoreCountFix = iniFile.GetBool("Tweaks", "HighCoreCountFix"); // 0 -> false
        bool serverBlocker = iniFile.GetBool("Tweaks", "ServerBlocker"); // 1 -> true

        // Assert
        Assert.That(highCoreCountFix, Is.False);
        Assert.That(serverBlocker, Is.True);
    }

    [Test]
    public void GetBool_WithNonIntegerValue_ReturnsDefaultValue()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        bool result = iniFile.GetBool("Metadata", "Version", true);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void GetDouble_WithValidDoubleValue_ReturnsCorrectValue()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        double logLevel = iniFile.GetDouble("Debug", "LogLevel");

        // Assert
        Assert.That(logLevel, Is.EqualTo(2.0));
    }

    [Test]
    public void GetDouble_WithNonDoubleValue_ReturnsDefaultValue()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        double result = iniFile.GetDouble("Metadata", "Version", 99.9);

        // Assert
        Assert.That(result, Is.EqualTo(99.9));
    }

    [Test]
    public void SetString_WithValidSectionAndKey_SetsValueCorrectly()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        iniFile.Set("TestSection", "TestKey", "TestValue");

        // Assert
        string result = iniFile.Get("TestSection", "TestKey");
        Assert.That(result, Is.EqualTo("TestValue"));
    }

    [Test]
    public void SetInt_WithValidSectionAndKey_SetsValueCorrectly()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        iniFile.Set("TestSection", "TestIntKey", 123);

        // Assert
        string result = iniFile.Get("TestSection", "TestIntKey");
        Assert.That(result, Is.EqualTo("123"));
    }

    [Test]
    public void SetBool_WithValidSectionAndKey_SetsValueCorrectly()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        iniFile.Set("TestSection", "TestBoolKey", true);

        // Assert
        string result = iniFile.Get("TestSection", "TestBoolKey");
        Assert.That(result, Is.EqualTo("1"));
    }

    [Test]
    public void SetBool_WithFalseValue_SetsValueToZero()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        iniFile.Set("TestSection", "TestBoolKey", false);

        // Assert
        string result = iniFile.Get("TestSection", "TestBoolKey");
        Assert.That(result, Is.EqualTo("0"));
    }

    [Test]
    public void SetDouble_WithValidSectionAndKey_SetsValueCorrectly()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        iniFile.Set("TestSection", "TestDoubleKey", 12.34);

        // Assert
        string result = iniFile.Get("TestSection", "TestDoubleKey");
        Assert.That(result, Is.EqualTo("12.34"));
    }

    [Test]
    public void HasSection_WithExistingSection_ReturnsTrue()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        bool result = iniFile.HasSection("Display");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void HasSection_WithNonExistentSection_ReturnsFalse()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        bool result = iniFile.HasSection("NonExistentSection");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void HasKey_WithExistingKey_ReturnsTrue()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        bool result = iniFile.HasKey("Display", "WindowMode");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void HasKey_WithNonExistentKey_ReturnsFalse()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        bool result = iniFile.HasKey("Display", "NonExistentKey");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void HasKey_WithNonExistentSection_ReturnsFalse()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        bool result = iniFile.HasKey("NonExistentSection", "AnyKey");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void RemoveKey_WithExistingKey_RemovesKeySuccessfully()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);
        iniFile.Set("TestSection", "TestKey", "TestValue");
        Assert.That(iniFile.HasKey("TestSection", "TestKey"), Is.True);

        // Act
        bool result = iniFile.RemoveKey("TestSection", "TestKey");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(iniFile.HasKey("TestSection", "TestKey"), Is.False);
    }

    [Test]
    public void RemoveKey_WithNonExistentKey_ReturnsFalse()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        bool result = iniFile.RemoveKey("Display", "NonExistentKey");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void RemoveSection_WithExistingSection_RemovesSectionSuccessfully()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);
        iniFile.Set("TestSection", "TestKey", "TestValue");
        Assert.That(iniFile.HasSection("TestSection"), Is.True);

        // Act
        bool result = iniFile.RemoveSection("TestSection");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(iniFile.HasSection("TestSection"), Is.False);
    }

    [Test]
    public void RemoveSection_WithNonExistentSection_ReturnsFalse()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        bool result = iniFile.RemoveSection("NonExistentSection");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void GetSections_ReturnsAllSectionNames()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        string[] sections = iniFile.GetSections();

        // Assert
        Assert.That(sections, Contains.Item("Metadata"));
        Assert.That(sections, Contains.Item("Display"));
        Assert.That(sections, Contains.Item("Tweaks"));
        Assert.That(sections, Contains.Item("Debug"));
        Assert.That(sections.Length, Is.EqualTo(4));
    }

    [Test]
    public void GetKeys_WithExistingSection_ReturnsAllKeyNames()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        string[] keys = iniFile.GetKeys("Display");

        // Assert
        Assert.That(keys, Contains.Item("WindowMode"));
        Assert.That(keys, Contains.Item("WindowWidth"));
        Assert.That(keys, Contains.Item("WindowHeight"));
        Assert.That(keys, Contains.Item("WindowPosX"));
        Assert.That(keys, Contains.Item("WindowPosY"));
        Assert.That(keys.Length, Is.EqualTo(5));
    }

    [Test]
    public void GetKeys_WithNonExistentSection_ReturnsEmptyArray()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act
        string[] keys = iniFile.GetKeys("NonExistentSection");

        // Assert
        Assert.That(keys.Length, Is.EqualTo(0));
    }

    [Test]
    public void Save_WithModifiedData_WritesToFileSuccessfully()
    {
        // Arrange - Copy the original file to a temporary file for testing
        File.Copy(TestIniFile, TempIniFile, true);
        IniFile iniFile = new IniFile(TempIniFile);

        // Act - Modify the data
        iniFile.Set("TestSection", "TestKey", "TestValue");
        iniFile.Set("Display", "WindowMode", "2");
        iniFile.Save();

        // Verify the file was saved by creating a new instance and checking the values
        IniFile iniFile2 = new IniFile(TempIniFile);
        
        // Assert
        Assert.That(iniFile2.Get("TestSection", "TestKey"), Is.EqualTo("TestValue"));
        Assert.That(iniFile2.Get("Display", "WindowMode"), Is.EqualTo("2"));

        // Cleanup
        if (File.Exists(TempIniFile))
        {
            File.Delete(TempIniFile);
        }
    }

    [Test]
    public void Get_WithNullSection_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.Get(null!, "Key"));
        Assert.That(exception.Message, Does.Contain("Section name cannot be null or empty"));
    }

    [Test]
    public void Get_WithEmptySection_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.Get("", "Key"));
        Assert.That(exception.Message, Does.Contain("Section name cannot be null or empty"));
    }

    [Test]
    public void Get_WithNullKey_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.Get("Section", null!));
        Assert.That(exception.Message, Does.Contain("Key name cannot be null or empty"));
    }

    [Test]
    public void Get_WithEmptyKey_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.Get("Section", ""));
        Assert.That(exception.Message, Does.Contain("Key name cannot be null or empty"));
    }

    [Test]
    public void Set_WithNullSection_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.Set(null!, "Key", "Value"));
        Assert.That(exception.Message, Does.Contain("Section name cannot be null or empty"));
    }

    [Test]
    public void Set_WithEmptySection_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.Set("", "Key", "Value"));
        Assert.That(exception.Message, Does.Contain("Section name cannot be null or empty"));
    }

    [Test]
    public void Set_WithNullKey_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.Set("Section", null!, "Value"));
        Assert.That(exception.Message, Does.Contain("Key name cannot be null or empty"));
    }

    [Test]
    public void Set_WithEmptyKey_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.Set("Section", "", "Value"));
        Assert.That(exception.Message, Does.Contain("Key name cannot be null or empty"));
    }

    [Test]
    public void HasSection_WithNullSection_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.HasSection(null!));
        Assert.That(exception.Message, Does.Contain("Section name cannot be null or empty"));
    }

    [Test]
    public void HasSection_WithEmptySection_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.HasSection(""));
        Assert.That(exception.Message, Does.Contain("Section name cannot be null or empty"));
    }

    [Test]
    public void HasKey_WithNullSection_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.HasKey(null!, "Key"));
        Assert.That(exception.Message, Does.Contain("Section name cannot be null or empty"));
    }

    [Test]
    public void HasKey_WithEmptySection_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.HasKey("", "Key"));
        Assert.That(exception.Message, Does.Contain("Section name cannot be null or empty"));
    }

    [Test]
    public void HasKey_WithNullKey_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.HasKey("Section", null!));
        Assert.That(exception.Message, Does.Contain("Key name cannot be null or empty"));
    }

    [Test]
    public void HasKey_WithEmptyKey_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.HasKey("Section", ""));
        Assert.That(exception.Message, Does.Contain("Key name cannot be null or empty"));
    }

    [Test]
    public void RemoveKey_WithNullSection_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.RemoveKey(null!, "Key"));
        Assert.That(exception.Message, Does.Contain("Section name cannot be null or empty"));
    }

    [Test]
    public void RemoveKey_WithEmptySection_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.RemoveKey("", "Key"));
        Assert.That(exception.Message, Does.Contain("Section name cannot be null or empty"));
    }

    [Test]
    public void RemoveKey_WithNullKey_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.RemoveKey("Section", null!));
        Assert.That(exception.Message, Does.Contain("Key name cannot be null or empty"));
    }

    [Test]
    public void RemoveKey_WithEmptyKey_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.RemoveKey("Section", ""));
        Assert.That(exception.Message, Does.Contain("Key name cannot be null or empty"));
    }

    [Test]
    public void RemoveSection_WithNullSection_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.RemoveSection(null!));
        Assert.That(exception.Message, Does.Contain("Section name cannot be null or empty"));
    }

    [Test]
    public void RemoveSection_WithEmptySection_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.RemoveSection(""));
        Assert.That(exception.Message, Does.Contain("Section name cannot be null or empty"));
    }

    [Test]
    public void GetKeys_WithNullSection_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.GetKeys(null!));
        Assert.That(exception.Message, Does.Contain("Section name cannot be null or empty"));
    }

    [Test]
    public void GetKeys_WithEmptySection_ThrowsArgumentException()
    {
        // Arrange
        IniFile iniFile = new IniFile(TestIniFile);

        // Act & Assert
        ArgumentException? exception = Assert.Throws<ArgumentException>(() => iniFile.GetKeys(""));
        Assert.That(exception.Message, Does.Contain("Section name cannot be null or empty"));
    }
}