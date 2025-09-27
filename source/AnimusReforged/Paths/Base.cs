namespace AnimusReforged.Paths;

public abstract class Base
{
    protected static string _basePath => AppDomain.CurrentDomain.BaseDirectory;
    protected static string AbsolutePath(string relativePath) => Path.Combine(_basePath, relativePath);
    protected static string AbsolutePath(params string[] parts) => Path.Combine(new[] { _basePath }.Concat(parts).ToArray());
}