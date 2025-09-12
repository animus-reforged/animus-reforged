namespace AnimusReforged.Utilities;

public static class ArgumentParser
{
    public static HashSet<string> ToArgumentSet(string[]? args) => args == null ? new HashSet<string>(StringComparer.OrdinalIgnoreCase) : new HashSet<string>(args, StringComparer.OrdinalIgnoreCase);

    public static bool Contains(string[]? args, string arg) => ToArgumentSet(args).Contains(arg);
}