using System;
using AnimusReforged.Altair.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using AnimusReforged.Altair.ViewModels;
using AnimusReforged.Logging;
using AnimusReforged.Settings;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;

namespace AnimusReforged.Altair.Views;

public partial class MainView : UserControl
{
    // Properties
    private MainViewModel _viewModel { get; set; }
    private AltairSettings _settings { get; set; }
    private readonly NavigationService _navigationService;

    // Constructor
    public MainView()
    {
        InitializeComponent();
        _viewModel = App.Services.GetRequiredService<MainViewModel>();
        DataContext = _viewModel;
        Loaded += async (_, _) =>
        {
            await _viewModel.CheckInstallation();
        };

        // Setup navigation service
        _navigationService = App.Services.GetRequiredService<NavigationService>();
        _navigationService.SetContentFrame(ContentFrame);
        _navigationService.SetNavigationView(NavView);
        
        _settings = App.Services.GetRequiredService<AltairSettings>();
        _viewModel.FirstSetup = _settings.Settings.SetupCompleted;
        _settings.SettingsChanged += (_, _) =>
        {
            if (!_settings.Settings.SetupCompleted)
            {
                _viewModel.FirstSetup = _settings.Settings.SetupCompleted;
                string targetPage = _settings.Settings.SetupCompleted ? "Default" : "Welcome";
                Dispatcher.UIThread.Post(() =>
                {
                    _navigationService.NavigateToTag(targetPage).Wait();
                });
            }
        };

        string initialPage = _settings.Settings.SetupCompleted ? "Default" : "Welcome";
        _navigationService.NavigateToTag(initialPage).Wait();
        
        Loaded += async (_, _) =>
        {
            if (_settings.Settings.SetupCompleted)
            {
                await _viewModel.CheckInstallation();
            }
        };
    }

    // Functions
    private async void NavView_OnItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        try
        {
            if (e.InvokedItemContainer is NavigationViewItem selectedItem)
            {
                await _navigationService.Navigate(selectedItem, ContentFrame);
            }
        }
        catch (Exception ex)
        {
            Logger.Error<MainView>("Failed to navigate to page");
            Logger.LogExceptionDetails<MainView>(ex);
        }
    }
}