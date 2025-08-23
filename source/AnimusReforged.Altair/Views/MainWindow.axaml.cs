using System;
using System.IO;
using System.Threading.Tasks;
using AnimusReforged.Altair.ViewModels;
using AnimusReforged.Paths;
using Avalonia.Interactivity;
using FluentAvalonia.UI.Controls;
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

    private void NavView_OnSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && e.SelectedItem is NavigationViewItem nvi)
        {
            vm.NavigateCommand.Execute(nvi.Content?.ToString() ?? "");
        }
    }
}