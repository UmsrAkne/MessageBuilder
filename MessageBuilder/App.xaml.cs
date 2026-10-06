using System.Windows;
using MessageBuilder.Services;
using MessageBuilder.Views;

namespace MessageBuilder;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        var toastService = new ToastService();
        containerRegistry.RegisterInstance(toastService);
        containerRegistry.Register<IClipboardWatchService, ClipboardWatcherService>();
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }
}