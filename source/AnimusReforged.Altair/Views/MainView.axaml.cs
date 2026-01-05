using AnimusReforged.Altair.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using AnimusReforged.Altair.ViewModels;
using FluentAvalonia.UI.Controls;

namespace AnimusReforged.Altair.Views;

public partial class MainView : UserControl
{
    // Properties
    private MainViewModel _viewModel { get; set; }
    private readonly NavigationService _navigationService;

    // Constructor
    public MainView()
    {
        InitializeComponent();
        _viewModel = App.Services.GetRequiredService<MainViewModel>();
        DataContext = _viewModel;

        // Setup navigation service
        _navigationService = App.Services.GetRequiredService<NavigationService>();
        _navigationService.SetContentFrame(ContentFrame);
        _navigationService.SetNavigationView(NavView);
        
    }

    // Functions
    private void NavView_OnItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.InvokedItemContainer is NavigationViewItem selectedItem)
        {
            _navigationService.Navigate(selectedItem, ContentFrame);
        }
    }
}