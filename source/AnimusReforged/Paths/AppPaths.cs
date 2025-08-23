namespace AnimusReforged.Paths;

public class AppPaths : Base
{
    public static readonly string Base = _basePath;
    public static readonly string ConfigFile = AbsolutePath("config.json");
    public static readonly string Downloads = AbsolutePath("Downloads");
    public static readonly string GameExecutable = AbsolutePath("AssassinsCreed_Dx9.exe");
    public static readonly string LogFile = AbsolutePath("animusreforged.log");
    public static readonly string Scripts = AbsolutePath("scripts");
    public static readonly string uMod = AbsolutePath("uMod");
}