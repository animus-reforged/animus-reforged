using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AnimusReforged.Altair.ViewModels;

public enum MessageBoxButtons
{
    Ok,
    OkCancel,
    YesNo,
    YesNoCancel
}

public partial class MessageBoxViewModel : ViewModelBase
{
    // Variables
    [ObservableProperty]
    private string message = string.Empty;

    public MessageBoxButtons Buttons { get; }
    
    public event EventHandler<bool>? RequestClose;

    // Constructor
    public MessageBoxViewModel() { }

    public MessageBoxViewModel(string message, MessageBoxButtons buttons = MessageBoxButtons.Ok, string? title = null)
    {
        Message = message;
        Buttons = buttons;
    }

    // Methods
    [RelayCommand]
    private void Ok() => CloseWithResult(true);

    [RelayCommand]
    private void Cancel() => CloseWithResult(false);

    [RelayCommand]
    private void Yes() => CloseWithResult(true);

    [RelayCommand]
    private void No() => CloseWithResult(false);

    private void CloseWithResult(bool result)
    {
        RequestClose?.Invoke(this, result);
    }
}