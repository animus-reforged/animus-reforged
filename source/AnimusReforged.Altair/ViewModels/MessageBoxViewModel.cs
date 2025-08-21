using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AnimusReforged.Altair.ViewModels;

public partial class MessageBoxViewModel : ViewModelBase
{
    [ObservableProperty]
    private string message = string.Empty;

    public MessageBoxViewModel() { }

    public MessageBoxViewModel(string message)
    {
        this.Message = message;
    }
     
    [RelayCommand]
    private void Ok()
    {
        RequestClose?.Invoke(this, true);
    }

    // Event the View can subscribe to
    public event EventHandler<bool>? RequestClose;
}