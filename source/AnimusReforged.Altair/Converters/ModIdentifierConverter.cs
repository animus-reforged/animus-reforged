using System;
using System.Globalization;
using AnimusReforged.Mods.Altair;
using Avalonia.Data.Converters;

namespace AnimusReforged.Altair.Converters;

public class ModIdentifierConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string modIdentifier)
        {
            return modIdentifier switch
            {
                ModIdentifiers.AsiLoader => "ASI Loader",
                ModIdentifiers.EaglePatch => "Eagle Patch",
                ModIdentifiers.AltairFix => "Altair Fix",
                ModIdentifiers.ReShade => "ReShade",
                ModIdentifiers.UMod => "UMod",
                ModIdentifiers.Overhaul => "Overhaul",
                _ => modIdentifier
            };
        }
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}