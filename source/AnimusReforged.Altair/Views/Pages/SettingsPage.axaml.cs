using AnimusReforged.Altair.ViewModels.Pages;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair.Views.Pages;

public partial class SettingsPage : UserControl
{
    private SettingsPageViewModel _viewModel { get; set; }

    public SettingsPage()
    {
        InitializeComponent();
        _viewModel = App.Services.GetRequiredService<SettingsPageViewModel>();
        DataContext = _viewModel;
    }
}