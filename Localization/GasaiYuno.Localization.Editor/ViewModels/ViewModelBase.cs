using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GasaiYuno.Localization.Editor.ViewModels;

public abstract class ViewModelBase<T> : ViewModelBase where T : ViewModelBase<T>
{
#if DEBUG
    public static T Design => App.Container.Resolve<T>();
#endif
}

public abstract class ViewModelBase : ObservableObject
{
    private string _title;

    /// <summary>Gets or sets the title of the View.</summary>
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
}