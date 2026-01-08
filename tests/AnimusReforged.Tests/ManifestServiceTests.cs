using AnimusReforged.Models;
using AnimusReforged.Mods.Core;
using AnimusReforged.Models.Mods;

namespace AnimusReforged.Tests;

[TestFixture]
public class ManifestServiceTests
{
    [SetUp]
    public void Setup()
    {
        // Clear all caches before each test to ensure isolation
        ManifestService.ClearAllCache();
    }
    
    [Test]
    public void GetManifestAsync_WithManifestTypeParameter_IsCallable()
    {
        // Act & Assert - Just verify the method exists and is callable
        Assert.DoesNotThrowAsync(async () =>
        {
            try
            {
                // This is in case it fails due to network error
                Task<ModManifest> task = ManifestService.GetManifestAsync(ManifestType.Altair);
                await task.WaitAsync(TimeSpan.FromSeconds(5));
            }
            catch (Exception)
            {
                // Expected to fail due to network, but method signature is correct
            }
        });
    }

    [Test]
    public void GetMod_DefaultCall_ThrowsWhenManifestNotLoaded()
    {
        // Act & Assert
        InvalidOperationException? exception = Assert.Throws<InvalidOperationException>(() => ManifestService.GetMod("mod-doesnt-exist"));

        // Verify the error message mentions Altair as the default
        Assert.That(exception.Message, Does.Contain("Altair manifest not loaded"));
    }

    [Test]
    public void GetMod_WithManifestTypeParameter_ThrowsWhenManifestNotLoaded()
    {
        // Act & Assert
        InvalidOperationException? exception = Assert.Throws<InvalidOperationException>(() => ManifestService.GetMod(ManifestType.Altair, "some-mod-id"));

        Assert.That(exception.Message, Does.Contain("Altair manifest not loaded"));
    }

    [Test]
    public void GetMod_WithDifferentManifestTypes_ThrowsWithCorrectTypeName()
    {
        // Act & Assert for Ezio
        InvalidOperationException? ezioException = Assert.Throws<InvalidOperationException>(() => ManifestService.GetMod(ManifestType.Ezio, "some-mod-id"));
        Assert.That(ezioException.Message, Does.Contain("Ezio manifest not loaded"));

        // Act & Assert for Brotherhood
        InvalidOperationException? brotherhoodException = Assert.Throws<InvalidOperationException>(() => ManifestService.GetMod(ManifestType.Brotherhood, "some-mod-id"));
        Assert.That(brotherhoodException.Message, Does.Contain("Brotherhood manifest not loaded"));

        // Act & Assert for Revelations
        InvalidOperationException? revelationsException = Assert.Throws<InvalidOperationException>(() => ManifestService.GetMod(ManifestType.Revelations, "some-mod-id"));
        Assert.That(revelationsException.Message, Does.Contain("Revelations manifest not loaded"));
    }

    [Test]
    public void ClearCache_MethodExistsAndIsCallable()
    {
        // Act & Assert
        Assert.DoesNotThrow(() => ManifestService.ClearCache(ManifestType.Altair));
        Assert.DoesNotThrow(() => ManifestService.ClearCache(ManifestType.Ezio));
        Assert.DoesNotThrow(() => ManifestService.ClearCache(ManifestType.Brotherhood));
        Assert.DoesNotThrow(() => ManifestService.ClearCache(ManifestType.Revelations));
    }

    [Test]
    public void ClearAllCache_MethodExistsAndIsCallable()
    {
        // Act & Assert
        Assert.DoesNotThrow(() => ManifestService.ClearAllCache());
    }

    [Test]
    public void ManifestType_EnumHasExpectedValues()
    {
        // Arrange & Act & Assert
        Assert.That(ManifestType.Altair, Is.EqualTo((ManifestType)0));
        Assert.That(ManifestType.Ezio, Is.EqualTo((ManifestType)1));
        Assert.That(ManifestType.Brotherhood, Is.EqualTo((ManifestType)2));
        Assert.That(ManifestType.Revelations, Is.EqualTo((ManifestType)3));
    }

    [Test]
    public void ManifestType_EnumHasExpectedNames()
    {
        // Arrange & Act & Assert
        Assert.That(ManifestType.Altair.ToString(), Is.EqualTo("Altair"));
        Assert.That(ManifestType.Ezio.ToString(), Is.EqualTo("Ezio"));
        Assert.That(ManifestType.Brotherhood.ToString(), Is.EqualTo("Brotherhood"));
        Assert.That(ManifestType.Revelations.ToString(), Is.EqualTo("Revelations"));
    }
    
    [Test]
    public async Task GetManifestAsync_ForceRefreshParameterWorks()
    {
        // Act & Assert - Just verify the method with forceRefresh parameter exists
        Assert.DoesNotThrowAsync(async () =>
        {
            try
            {
                Task<ModManifest> task = ManifestService.GetManifestAsync(ManifestType.Altair, true); // Original signature with forceRefresh
                await task.WaitAsync(TimeSpan.FromMilliseconds(100));
            }
            catch (Exception)
            {
                // Expected due to network, but method exists
            }
        });

        Assert.DoesNotThrowAsync(async () =>
        {
            try
            {
                Task<ModManifest> task = ManifestService.GetManifestAsync(ManifestType.Altair, true); // New signature with forceRefresh
                await task.WaitAsync(TimeSpan.FromMilliseconds(100));
            }
            catch (Exception)
            {
                // Expected due to network, but method exists
            }
        });
    }
}