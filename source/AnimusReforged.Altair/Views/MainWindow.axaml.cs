using AnimusReforged.Altair.ViewModels;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair.Views;

public partial class MainWindow : Window
{
    // Properties
    private MainWindowViewModel _viewModel { get; set; }
    
    public MainWindow()
    {
        InitializeComponent();
        _viewModel = App.Services.GetRequiredService<MainWindowViewModel>();
        DataContext = _viewModel;
    }
}