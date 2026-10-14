using System.IO;
using System.Windows;
using MessageBuilder.Services;
using MessageBuilder.ViewModels;
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
        containerRegistry.RegisterSingleton<IClipboardWatchService, ClipboardWatcherService>();
        containerRegistry.Register<TextListSerializer>();
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // 1. MainWindow の DataContext から MainWindowViewModel を取得
        if (MainWindow?.DataContext is not MainWindowViewModel mainWindowViewModel)
        {
            return;
        }

        // 2. Serializer の取得（DIコンテナから解決）
        var serializer = Container.Resolve<TextListSerializer>();
        var targetDirectory = Path.Combine(AppContext.BaseDirectory, "text_list");

        if (!Directory.Exists(targetDirectory))
        {
            return;
        }

        var jsonFiles = Directory.GetFiles(targetDirectory, "*.json");
        var list = jsonFiles.OrderByDescending(x => x)
            .Take(5);

        foreach (var filePath in list)
        {
            var textListVm = Container.Resolve<TextListViewModel>();
            serializer.DeserializeInto(filePath, textListVm);
            textListVm.SourceFilePath = filePath;
            mainWindowViewModel.TextListViewModels.Add(textListVm);
        }
    }
}