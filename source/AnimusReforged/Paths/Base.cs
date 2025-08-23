namespace AnimusReforged.Paths;

public abstract class Base
{
    protected static string _basePath => AppDomain.CurrentDomain.BaseDirectory;
    protected static string AbsolutePath(string relativePath) => Path.Combine(_basePath, relativePath);
}