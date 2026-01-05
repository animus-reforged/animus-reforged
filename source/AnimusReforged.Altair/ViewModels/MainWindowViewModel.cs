using CommunityToolkit.Mvvm.ComponentModel;

namespace AnimusReforged.Altair.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private bool disableWindow;

    public MainWindowViewModel()
    {
        disableWindow = false;
    }
}