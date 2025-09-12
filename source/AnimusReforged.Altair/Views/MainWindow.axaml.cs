using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AnimusReforged.Altair.ViewModels;
using AnimusReforged.Altair.Views.Pages;
using AnimusReforged.Paths;
using Avalonia.Controls;
using Avalonia.Interactivity;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using FluentAvalonia.UI.Windowing;

namespace AnimusReforged.Altair.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            string initialPage = vm.DisableWindow ? "Default" : "Welcome";
            NavigateToPage(initialPage);
            vm.NavigationRequested += (_, destination) => NavigateToPage(destination);
        }
        try
        {
#if !DEBUG
            await CheckInstallation();
#else
            await Task.CompletedTask;
#endif
        }
        catch (Exception ex)
        {
            Logger.Error($"Error during startup");
            Logger.LogExceptionDetails(ex);
            Environment.Exit(0);
        }
    }

    private async Task CheckInstallation()
    {
        Logger.Info("Checking if the AnimusReforged has been placed next to the game executable");
        if (!File.Exists(AppPaths.AltairGameExecutable))
        {
            Logger.Error("Missing game executable, terminating");
            await MessageBox.ShowAsync($"Game executable not found. Please make sure you have the game installed and the executable is in the game folder next to the {Path.GetFileName(AppPaths.AltairGameExecutable)}.", "Error", App.MainWindow);
            Environment.Exit(0);
        }
    }

    private void NavView_OnItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        NavigateToPage(e.InvokedItemContainer?.Content?.ToString());
    }
    
    private async void NavigateToPage(string? contentTag)
    {
        if (string.IsNullOrEmpty(contentTag))
        {
            return;
        }

        switch (contentTag)
        {
            case "Welcome":
                ContentFrame.Navigate(typeof(WelcomePage), null, null);
                break;
            case "Play":
                ContentFrame.Navigate(typeof(DefaultPage), null, new DrillInNavigationTransitionInfo());
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.DisableWindow = false;
                    await AnimusReforged.Launcher.Altair.LaunchAsync(App.Settings.Tweaks.UMod);
                    vm.DisableWindow = true;
                }
                break;
            case "Settings":
                ContentFrame.Navigate(typeof(SettingsPage), null, new EntranceNavigationTransitionInfo());
                break;
            case "Credits":
                ContentFrame.Navigate(typeof(CreditsPage), null, new EntranceNavigationTransitionInfo());
                break;
            case "Donate":
                await MessageBox.ShowAsync("Donations are and will always be optional.\nEverything made by me on my own in my spare time will always be free and fully open source.\nDon't donate unless you can really afford it and don't donate your parents money without them knowing.\nDonations are NOT refundable.", "Information");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://ko-fi.com/shazzaam/",
                    UseShellExecute = true
                });
                break;
            default:
                ContentFrame.Navigate(typeof(DefaultPage), null, null);
                break;
        }
    }
}