using System;
using System.Threading.Tasks;
using AnimusReforged.Altair.ViewModels;
using Avalonia.Controls;
using FluentAvalonia.UI.Windowing;

namespace AnimusReforged.Altair.Views;

public partial class MessageBox : AppWindow
{
    // Variables
    private Button? _okButton;
    private Button? _cancelButton;
    private Button? _yesButton;
    private Button? _noButton;

    // Constructors
    public MessageBox()
    {
        InitializeComponent();
        TitleBar.Height = 0;

        _okButton = this.FindControl<Button>("OkButton");
        _cancelButton = this.FindControl<Button>("CancelButton");
        _yesButton = this.FindControl<Button>("YesButton");
        _noButton = this.FindControl<Button>("NoButton");
    }

    public MessageBox(string message, MessageBoxButtons buttons = MessageBoxButtons.Ok, string title = "Message") : this()
    {
        MessageBoxViewModel viewModel = new MessageBoxViewModel(message, buttons);
        viewModel.RequestClose += (_, result) => Close(result);
        DataContext = viewModel;
        Title = title;

        UpdateButtonVisibility(viewModel.Buttons);
    }

    // Methods
    private void UpdateButtonVisibility(MessageBoxButtons buttons)
    {
        _okButton?.SetCurrentValue(IsVisibleProperty, buttons == MessageBoxButtons.Ok || buttons == MessageBoxButtons.OkCancel);
        _cancelButton?.SetCurrentValue(IsVisibleProperty, buttons == MessageBoxButtons.OkCancel || buttons == MessageBoxButtons.YesNoCancel);
        _yesButton?.SetCurrentValue(IsVisibleProperty, buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel);
        _noButton?.SetCurrentValue(IsVisibleProperty, buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is MessageBoxViewModel vm)
        {
            UpdateButtonVisibility(vm.Buttons);
        }
    }

    public static void Show(string message, MessageBoxButtons buttons = MessageBoxButtons.Ok, string title = "Message", Window? owner = null)
    {
        MessageBox messageBox = new MessageBox(message, buttons, title);
        if (owner != null)
        {
            messageBox.ShowDialog(owner);
        }
        else
        {
            Window? mainWindow = App.MainWindow;
            if (mainWindow != null)
            {
                messageBox.ShowDialog(mainWindow);
            }
            else
            {
                messageBox.Show();
            }
        }
    }

    public static async Task<bool> ShowAsync(string message, MessageBoxButtons buttons = MessageBoxButtons.Ok, string title = "Message", Window? owner = null)
    {
        MessageBox messageBox = new MessageBox(message, buttons, title);
        if (owner != null)
        {
            return await messageBox.ShowDialog<bool>(owner);
        }
        Window? mainWindow = App.MainWindow;
        if (mainWindow != null)
        {
            return await messageBox.ShowDialog<bool>(mainWindow);
        }
        messageBox.Show();
        return true;
    }
}