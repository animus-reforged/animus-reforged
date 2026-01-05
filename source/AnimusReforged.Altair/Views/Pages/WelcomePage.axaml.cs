using AnimusReforged.Altair.ViewModels.Pages;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair.Views.Pages;

public partial class WelcomePage : UserControl
{
    private WelcomePageViewModel _viewModel { get; set; }
    
    public WelcomePage()
    {
        InitializeComponent();
        _viewModel = App.Services.GetRequiredService<WelcomePageViewModel>();
        DataContext = _viewModel;
    }
}