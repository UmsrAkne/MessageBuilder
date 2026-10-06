using System.Diagnostics;
using MessageBuilder.Services;
using MessageBuilder.Utils;

namespace MessageBuilder.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private string title = "MessageBuilder";
        private readonly IClipboardWatchService clipboardWatchService;
        private bool isClipboardWatching;

        public MainWindowViewModel()
        {
            TextListViewModel = new TextListViewModel(new ToastService());
            clipboardWatchService = new ClipboardWatcherService();
            AppLogger.Info("MainWindowViewModel created (default)");
            SetDummyData();
        }

        public MainWindowViewModel(TextListViewModel textListViewModel, ToastService toastService, IClipboardWatchService clipboardWatchService)
        {
            ToastService = toastService;
            TextListViewModel = textListViewModel;
            AppLogger.Info("MainWindowViewModel created (DI)");
            this.clipboardWatchService = clipboardWatchService;
            this.clipboardWatchService.TextCopied += OnTextCopied;
            SetDummyData();
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public ToastService ToastService { get; set; } = new ();

        public TextListViewModel TextListViewModel { get; set; }

        public bool IsClipboardWatching
        {
            get => isClipboardWatching;
            set => SetProperty(ref isClipboardWatching, value);
        }

        private void OnTextCopied(object? sender, string text)
        {
            if (!IsClipboardWatching)
            {
                return;
            }

            var t = text.Trim();
            TextListViewModel.TextListItems.Add(new TextListItem(t));
        }

        [Conditional("DEBUG")]
        private void SetDummyData()
        {
            TextListViewModel.TextListItems.Add(new TextListItem("Dummy Text 1111") { Info = "lang info1", IsCodeBlock = true, });
            TextListViewModel.TextListItems.Add(new TextListItem("Dummy Text 2222") { Info = "lang info2", IsCodeBlock = true, });

            TextListViewModel.TextListItems.Add(new TextListItem("Dummy Text 2222") { IsCodeBlock = false, });

            TextListViewModel.TextListItems.Add(new TextListItem("Dummy Text 3333") { Info = "lang info3", IsCodeBlock = true, });

            TextListViewModel.TextListItems.Add(new TextListItem("Dummy Text 4444") { Info = "lang info4", IsCodeBlock = true, IsIncluded = false, });
            TextListViewModel.TextListItems.Add(new TextListItem("Dummy Text 5555") { Info = "lang info5", IsCodeBlock = false, IsIncluded = false, });
        }
    }
}