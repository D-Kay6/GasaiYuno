using System.Linq;
using System.Reflection;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using GasaiYuno.Localization.Editor.ViewModels;
using GasaiYuno.Localization.Editor.Views;

namespace GasaiYuno.Localization.Editor;

public partial class App : Application
{
    public static IContainer Container { get; private set; }

    public App()
    {
        Container = new ContainerBuilder().Build();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var builder = new ContainerBuilder();
        RegisterComponents(builder);
        Container = builder.Build();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow
            {
                DataContext = Container.Resolve<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void RegisterComponents(ContainerBuilder builder)
    {
        Assembly.GetExecutingAssembly().GetTypes().Where(x => !x.IsAbstract && (
            x.IsSubclassOf(typeof(ViewModelBase)) ||
            x.IsSubclassOf(typeof(UserControl))
        )).ToList().ForEach(x =>
            builder.RegisterType(x).AsSelf().AsImplementedInterfaces().InstancePerDependency()
        );
    }
}