using System;
using System.IO;
using System.Threading.Tasks;
using AnimusReforged.Altair.Services.UI;
using AnimusReforged.Logging;
using AnimusReforged.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IMessageBoxService _messageBoxService;
    [ObservableProperty] private bool firstSetup;
    
    public MainViewModel()
    {
        _messageBoxService = App.Services.GetRequiredService<IMessageBoxService>();
    }

    public async Task CheckInstallation()
    {
        Logger.Info<MainViewModel>("Checking if the AnimusReforged has been placed next to the game executable");
        if (!File.Exists(FilePaths.AltairExecutable))
        {
            Logger.Error<MainViewModel>("Missing game executable");
            await _messageBoxService.ShowErrorAsync($"Game executable not found.", $"Please make sure you have the game installed and the executable is in the game folder next to the {Path.GetFileName(FilePaths.AltairExecutable)}.");
            Environment.Exit(0);
        }
    }
}