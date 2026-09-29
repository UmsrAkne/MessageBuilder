using System.Windows;
using Prism.DryIoc;
using Prism.Ioc;
using MessageBuilder.Views;

namespace MessageBuilder;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }
}