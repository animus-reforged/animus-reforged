using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AnimusReforged.Altair.ViewModels;

namespace AnimusReforged.Altair;

public class ViewLocator : IDataTemplate
{
    private static readonly (string From, string To)[] NamespaceReplacements =
    {
        (".ViewModels.", ".Views."),
        (".ViewModels.Pages.", ".Views.Pages."),
        (".ViewModels.Windows.", ".Views.Windows.")
    };

    public Control? Build(object? param)
    {
        if (param is null)
        {
            return null;
        }

        var vmType = param.GetType();
        var vmFullName = vmType.FullName!;

        foreach ((string from, string to) in NamespaceReplacements)
        {
            var candidate = vmFullName.Replace(from, to, StringComparison.Ordinal)
                .Replace("ViewModel", "View", StringComparison.Ordinal);

            var type = Type.GetType(candidate);
            if (type != null)
                return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = $"Not Found for {vmFullName}" };
    }

    public bool Match(object? data) => data is ViewModelBase;
}