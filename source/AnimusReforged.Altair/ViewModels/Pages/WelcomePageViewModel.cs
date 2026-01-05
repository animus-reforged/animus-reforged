using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AnimusReforged.Altair.ViewModels.Pages;

public partial class WelcomePageViewModel : ViewModelBase
{
    // Variables
    [ObservableProperty] private string statusText = string.Empty;

    [ObservableProperty] private int progressBarValue;

    // Methods
    [RelayCommand]
    private async Task Install()
    {
        
    }
}