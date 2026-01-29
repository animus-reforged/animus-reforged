using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using AnimusReforged.Logging;
using CommunityToolkit.Mvvm.Input;
using FluentIcons.Common;

namespace AnimusReforged.Altair.ViewModels.Pages;

public partial class CreditsPageViewModel : ViewModelBase
{
    public class CreditItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public Symbol Icon { get; set; } = Symbol.Link;
    }

    public ObservableCollection<CreditItem> Credits { get; } =
    [
        new CreditItem
        {
            Title = "uMod Tool",
            Description = "For allowing texture modifications",
            Url = "https://code.google.com/archive/p/texmod/",
            Icon = Symbol.Wrench
        },

        new CreditItem
        {
            Title = "hecumarine",
            Description = "For creating Overhaul Mod",
            Url = "https://www.moddb.com/mods/assassins-creed-2014-overhaul",
            Icon = Symbol.Sparkle
        },

        new CreditItem
        {
            Title = "Sergeanur",
            Description = "For creating EaglePatch",
            Url = "https://github.com/Sergeanur/EaglePatch",
            Icon = Symbol.Code
        },

        new CreditItem
        {
            Title = "crosire",
            Description = "For creating ReShade",
            Url = "https://reshade.me/",
            Icon = Symbol.ColorBackground
        },

        new CreditItem
        {
            Title = "Ciocolici",
            Description = "For the ReShade Preset",
            Url = "https://steamcommunity.com/sharedfiles/filedetails/?id=2957930769",
            Icon = Symbol.PaintBrush
        },

        new CreditItem
        {
            Title = "Claudio",
            Description = "For creating banner and upscaling background image",
            Url = "https://www.nexusmods.com/users/70787823",
            Icon = Symbol.ImageSparkle
        }
    ];

    [RelayCommand]
    private void OpenLink(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Logger.Warning<CreditsPageViewModel>("No URL provided to open");
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Logger.Error<CreditsPageViewModel>($"There was an error opening the link: {url}");
            Logger.LogExceptionDetails<CreditsPageViewModel>(ex);
        }
    }
}