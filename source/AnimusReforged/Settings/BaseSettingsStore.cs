using System.Reflection;

namespace AnimusReforged.Settings;

public abstract class BaseSettingsStore
{
    public string GetVersion()
    {
        try
        {
            Assembly? assembly = Assembly.GetEntryAssembly();
            var version = assembly?.GetName()?.Version;
            if (version == null)
            {
                return "0.0.0";
            }
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
        catch
        {
            return "0.0.0";
        }
    }
}