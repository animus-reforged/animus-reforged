using AnimusReforged.Altair.ViewModels.Pages;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair.Views.Pages;

public partial class ManagePage : UserControl
{
    private ManagePageViewModel _viewModel { get; set; }

    public ManagePage()
    {
        InitializeComponent();
        _viewModel = App.Services.GetRequiredService<ManagePageViewModel>();
        DataContext = _viewModel;
        Loaded += (_, _) =>
        {
            _viewModel.OnPageLoaded();
        };
    }
}