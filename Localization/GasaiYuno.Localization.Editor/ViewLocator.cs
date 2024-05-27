using System;
using Autofac;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using GasaiYuno.Localization.Editor.ViewModels;

namespace GasaiYuno.Localization.Editor;

public class ViewLocator : IDataTemplate
{
    public Control Build(object data)
    {
        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)App.Container.Resolve(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object data)
    {
        return data is ViewModelBase;
    }
}