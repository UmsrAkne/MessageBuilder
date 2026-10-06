using System.Windows;
using MessageBuilder.Services;
using MessageBuilder.Utils;

namespace MessageBuilder.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly IClipboardWatchService? clipboardService;

    public MainWindow()
    {
        InitializeComponent();
        AppLogger.Info("MainWindow is created");
    }

    public MainWindow(IClipboardWatchService clipboardService)
    {
        InitializeComponent();
        AppLogger.Info("MainWindow is created (DI)");
        this.clipboardService = clipboardService;

        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        clipboardService?.Start(this);
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        clipboardService?.Stop();
    }
}