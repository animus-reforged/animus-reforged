using AnimusReforged.Utilities;

namespace AnimusReforged.Tests;

[TestFixture]
public class ArgumentParserTests
{
    [Test]
    public void ToArgumentSet_WithNullArgs_ReturnsEmptyHashSet()
    {
        // Arrange
        string[]? args = null;

        // Act
        HashSet<string> result = ArgumentParser.ToArgumentSet(args);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
        Assert.That(result.Comparer, Is.InstanceOf<StringComparer>());
        Assert.That(((StringComparer)result.Comparer).Compare("test", "TEST"), Is.EqualTo(0)); // Case insensitive
    }

    [Test]
    public void ToArgumentSet_WithEmptyArgs_ReturnsEmptyHashSet()
    {
        // Arrange
        string[] args = Array.Empty<string>();

        // Act
        HashSet<string> result = ArgumentParser.ToArgumentSet(args);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
        Assert.That(result.Comparer, Is.InstanceOf<StringComparer>());
        Assert.That(((StringComparer)result.Comparer).Compare("test", "TEST"), Is.EqualTo(0)); // Case insensitive
    }

    [Test]
    public void ToArgumentSet_WithSingleArg_ReturnsHashSetWithOneElement()
    {
        // Arrange
        string[] args = { "test" };

        // Act
        HashSet<string> result = ArgumentParser.ToArgumentSet(args);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Contains("test"), Is.True);
        Assert.That(result.Contains("TEST"), Is.True); // Case insensitive
    }

    [Test]
    public void ToArgumentSet_WithMultipleArgs_ReturnsHashSetWithAllElements()
    {
        // Arrange
        string[] args = { "arg1", "arg2", "arg3" };

        // Act
        HashSet<string> result = ArgumentParser.ToArgumentSet(args);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Contains("arg1"), Is.True);
        Assert.That(result.Contains("arg2"), Is.True);
        Assert.That(result.Contains("arg3"), Is.True);
        Assert.That(result.Contains("ARG1"), Is.True); // Case insensitive
        Assert.That(result.Contains("ARG2"), Is.True); // Case insensitive
        Assert.That(result.Contains("ARG3"), Is.True); // Case insensitive
    }

    [Test]
    public void ToArgumentSet_WithDuplicateArgs_ReturnsHashSetWithUniqueElements()
    {
        // Arrange
        string[] args = { "arg1", "arg2", "arg1", "ARG1", "Arg2" };

        // Act
        HashSet<string> result = ArgumentParser.ToArgumentSet(args);

        // Assert
        Assert.That(result, Is.Not.Null);
        // Since the comparer is case-insensitive, "arg1", "ARG1", and "Arg1" are considered the same
        // So we expect 2 unique elements: "arg1" (case-insensitive) and "arg2" (case-insensitive)
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Contains("arg1"), Is.True);
        Assert.That(result.Contains("arg2"), Is.True);
        Assert.That(result.Contains("ARG1"), Is.True); // Case insensitive
        Assert.That(result.Contains("ARG2"), Is.True); // Case insensitive
    }

    [Test]
    public void ToArgumentSet_WithEmptyStringArg_ReturnsHashSetWithEmptyString()
    {
        // Arrange
        string[] args = { "" };

        // Act
        HashSet<string> result = ArgumentParser.ToArgumentSet(args);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Contains(""), Is.True);
    }

    [Test]
    public void ToArgumentSet_WithWhitespaceArgs_ReturnsHashSetWithWhitespaceStrings()
    {
        // Arrange
        string[] args = { " ", "  ", "\t", "\n" };

        // Act
        HashSet<string> result = ArgumentParser.ToArgumentSet(args);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(4));
        Assert.That(result.Contains(" "), Is.True);
        Assert.That(result.Contains("  "), Is.True);
        Assert.That(result.Contains("\t"), Is.True);
        Assert.That(result.Contains("\n"), Is.True);
    }

    [Test]
    public void Contains_WithNullArgsAndAnyArg_ReturnsFalse()
    {
        // Arrange
        string[]? args = null;
        string arg = "test";

        // Act
        bool result = ArgumentParser.Contains(args, arg);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Contains_WithEmptyArgsAndAnyArg_ReturnsFalse()
    {
        // Arrange
        string[] args = Array.Empty<string>();
        string arg = "test";

        // Act
        bool result = ArgumentParser.Contains(args, arg);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Contains_WithArgsThatContainArg_ReturnsTrue()
    {
        // Arrange
        string[] args = { "arg1", "test", "arg3" };
        string arg = "test";

        // Act
        bool result = ArgumentParser.Contains(args, arg);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_WithArgsThatDoNotContainArg_ReturnsFalse()
    {
        // Arrange
        string[] args = { "arg1", "arg2", "arg3" };
        string arg = "test";

        // Act
        bool result = ArgumentParser.Contains(args, arg);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Contains_WithCaseInsensitiveMatch_ReturnsTrue()
    {
        // Arrange
        string[] args = { "Arg1", "TEST", "arg3" };
        string arg = "test";

        // Act
        bool result = ArgumentParser.Contains(args, arg);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_WithCaseInsensitiveMatchInReverse_ReturnsTrue()
    {
        // Arrange
        string[] args = { "arg1", "test", "arg3" };
        string arg = "TEST";

        // Act
        bool result = ArgumentParser.Contains(args, arg);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_WithEmptyStringArgInArgs_ReturnsTrueWhenSearchingForEmptyString()
    {
        // Arrange
        string[] args = { "arg1", "", "arg3" };
        string arg = "";

        // Act
        bool result = ArgumentParser.Contains(args, arg);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_WithEmptyStringArgNotInArgs_ReturnsFalseWhenSearchingForEmptyString()
    {
        // Arrange
        string[] args = { "arg1", "arg2", "arg3" };
        string arg = "";

        // Act
        bool result = ArgumentParser.Contains(args, arg);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Contains_WithWhitespaceArgInArgs_ReturnsTrueWhenSearchingForSameWhitespace()
    {
        // Arrange
        string[] args = { "arg1", " ", "arg3" };
        string arg = " ";

        // Act
        bool result = ArgumentParser.Contains(args, arg);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_WithDifferentCasingOfSameWord_ReturnsTrue()
    {
        // Arrange
        string[] args = { "hello", "world" };
        string arg = "HELLO";

        // Act
        bool result = ArgumentParser.Contains(args, arg);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_WithSpecialCharacters_ReturnsTrueWhenMatchExists()
    {
        // Arrange
        string[] args = { "--verbose", "/debug", "-h" };
        string arg = "--VERBOSE"; // Different case

        // Act
        bool result = ArgumentParser.Contains(args, arg);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_WithPartialMatch_ReturnsFalse()
    {
        // Arrange
        string[] args = { "hello", "world" };
        string arg = "hell"; // Partial match

        // Act
        bool result = ArgumentParser.Contains(args, arg);

        // Assert
        Assert.That(result, Is.False);
    }
}