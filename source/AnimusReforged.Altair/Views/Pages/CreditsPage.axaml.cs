using AnimusReforged.Altair.ViewModels.Pages;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair.Views.Pages;

public partial class CreditsPage : UserControl
{
    private CreditsPageViewModel _viewModel { get; set; }

    public CreditsPage()
    {
        InitializeComponent();
        _viewModel = App.Services.GetRequiredService<CreditsPageViewModel>();
        DataContext = _viewModel;
    }
}