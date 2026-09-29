using MessageBuilder.Utils;

namespace MessageBuilder.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private string title = "MessageBuilder";

        public MainWindowViewModel()
        {
            AppLogger.Info("MainWindowViewModel created");
        }

        public string Title { get => title; set => SetProperty(ref title, value); }
    }
}