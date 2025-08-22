using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AnimusReforged.Altair.ViewModels;

public partial class WelcomePageViewModel : ViewModelBase
{
    // Variables
    [ObservableProperty] 
    private string statusText = string.Empty;
    
    // Methods
    [RelayCommand]
    private void Install()
    {
        Logger.Debug("Installing AnimusReforged mods");
    }
}