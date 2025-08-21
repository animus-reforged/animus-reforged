using System.Threading.Tasks;
using AnimusReforged.Altair.ViewModels;
using Avalonia.Controls;
using FluentAvalonia.UI.Windowing;

namespace AnimusReforged.Altair.Views;

public partial class MessageBox : AppWindow
{
    public MessageBox()
    {
        InitializeComponent();
        TitleBar.Height = 0;
    }

    public MessageBox(string message, string title = "Message") : this()
    {
        MessageBoxViewModel viewModel = new MessageBoxViewModel(message);
        viewModel.RequestClose += (_, result) => Close(result);
        DataContext = viewModel;
        Title = title;
    }

    // Static method for easy usage throughout the app
    public static void Show(string message, string title = "Message", Window? owner = null)
    {
        MessageBox messageBox = new MessageBox(message, title);
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
                messageBox.Show(); // Fallback to non-modal if no main window
            }
        }
    }

    // Async version for when you need to wait for the result
    public static async Task<bool> ShowAsync(string message, string title = "Message", Window? owner = null)
    {
        MessageBox messageBox = new MessageBox(message, title);
        if (owner != null)
        {
            return await messageBox.ShowDialog<bool>(owner);
        }
        else
        {
            Window? mainWindow = App.MainWindow;
            if (mainWindow != null)
            {
                return await messageBox.ShowDialog<bool>(mainWindow);
            }
            else
            {
                messageBox.Show(); // Fallback to non-modal if no main window
                return true; // Always returns true for non-dialog mode
            }
        }
    }
}