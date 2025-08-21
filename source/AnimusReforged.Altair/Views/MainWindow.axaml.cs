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
        Loaded += async (_, _) =>
        {
            if (!File.Exists(AppPaths.GameExecutable))
            {
                await MessageBox.ShowAsync("Game executable not found. Please make sure you have the game installed and the executable is in the correct location.", "Error", App.MainWindow);
                Environment.Exit(0);
            }
        };
    }
    
    private void NavView_OnSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && e.SelectedItem is NavigationViewItem nvi)
        {
            vm.NavigateCommand.Execute(nvi.Content?.ToString() ?? "");
        }
    }
}