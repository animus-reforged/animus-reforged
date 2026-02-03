using System;
using System.Threading.Tasks;
using AnimusReforged.Altair.Services;
using AnimusReforged.Altair.ViewModels;
using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair.Views;

public partial class MainWindow : Window
{
    // Properties
    private MainWindowViewModel _viewModel { get; set; }
    private readonly IUpdateNotificationService _updateNotificationService;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = App.Services.GetRequiredService<MainWindowViewModel>();
        _updateNotificationService = App.Services.GetRequiredService<IUpdateNotificationService>();
        DataContext = _viewModel;
    }

    private void UpdateNotification_CloseButtonClick(InfoBar infoBar, EventArgs args)
    {
        _viewModel.ClearUpdateNotifications();
    }
}