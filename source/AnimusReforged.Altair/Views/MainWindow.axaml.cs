using System;
using System.IO;
using AnimusReforged.Altair.ViewModels;
using AnimusReforged.Paths;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;

namespace AnimusReforged.Altair.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        InitializeComponent();
#if !DEBUG
        Loaded += async (_, _) =>
        {
            Logger.Info("Checking if the AnimusReforged has been placed next to the game executable");
            if (!File.Exists(AppPaths.GameExecutable))
            {
                Logger.Error("Missing game executable, terminating");
                await MessageBox.ShowAsync($"Game executable not found. Please make sure you have the game installed and the executable is in the game folder next to the {AppPaths.GameExecutable}.", "Error", App.MainWindow);
                Environment.Exit(0);
            }
        };
#endif
    }

    private void NavView_OnSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && e.SelectedItem is NavigationViewItem nvi)
        {
            vm.NavigateCommand.Execute(nvi.Content?.ToString() ?? "");
        }
    }
}