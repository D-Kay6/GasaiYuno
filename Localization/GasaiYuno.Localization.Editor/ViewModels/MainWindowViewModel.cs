namespace GasaiYuno.Localization.Editor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase<MainWindowViewModel>
{
    public ViewModelBase Modules { get; }

    public MainWindowViewModel(ModulesViewModel modules)
    {
        Modules = modules;
        Title = "Gasai Yuno Localization Editor";
    }
}