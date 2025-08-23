namespace AnimusReforged.Paths;

public class AppPaths : Base
{
    public static readonly string Base = _basePath;
    public static readonly string ConfigFile = AbsolutePath("config.json");
    public static readonly string Downloads = AbsolutePath("Downloads");
    public static readonly string AltairGameExecutable = AbsolutePath("AssassinsCreed_Dx9.exe");
    public static readonly string LogFile = AbsolutePath("animusreforged.log");
    public static readonly string Scripts = AbsolutePath("scripts");
    public static readonly string uMod = AbsolutePath("uMod");
    public static readonly string uModAppdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "uMod");
    public static readonly string uModTemplates = Path.Combine(uMod, "templates");
    public static readonly string uModSaveFiles = Path.Combine(uMod, "uMod_SaveFiles.txt");
    public static readonly string uModStatusFile = Path.Combine(uMod, "Status.txt");
    public static readonly string uModConfig = Path.Combine(uModAppdata, "uMod_DX9.txt");
    public static readonly string Mods = AbsolutePath("mods");
    public static readonly string AltairOverhaulMod = Path.Combine(Mods, "Overhaul");
    public static readonly string AltairOverhaulModFile = Path.Combine(AltairOverhaulMod, "Overhaul.tpf");
}