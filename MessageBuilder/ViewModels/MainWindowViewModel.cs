using System.Collections.ObjectModel;
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
        private readonly Func<TextListViewModel>? textListViewModelFactory;
        private bool isClipboardWatching;
        private TextListViewModel? selectedTab;

        public MainWindowViewModel()
        {
            TextListViewModels.Add(new TextListViewModel(new ToastService()));
            clipboardWatchService = new ClipboardWatcherService();
            textListViewModelFactory = null;
            AppLogger.Info("MainWindowViewModel created (default)");
            SetDummyData();
        }

        public MainWindowViewModel(Func<TextListViewModel> textListViewModelFactory, ToastService toastService, IClipboardWatchService clipboardWatchService)
        {
            ToastService = toastService;
            this.textListViewModelFactory = textListViewModelFactory;
            TextListViewModels.Add(textListViewModelFactory());
            AppLogger.Info("MainWindowViewModel created (DI)");
            this.clipboardWatchService = clipboardWatchService;
            this.clipboardWatchService.TextCopied += OnTextCopied;
            SetDummyData();
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public ToastService ToastService { get; set; } = new ();

        public ObservableCollection<TextListViewModel> TextListViewModels { get; set; } = new ();

        public bool IsClipboardWatching
        {
            get => isClipboardWatching;
            set => SetProperty(ref isClipboardWatching, value);
        }

        public TextListViewModel? SelectedTab
        {
            get => selectedTab;
            set => SetProperty(ref selectedTab, value);
        }

        private void OnTextCopied(object? sender, string text)
        {
            if (!IsClipboardWatching || SelectedTab == null)
            {
                return;
            }

            // ガードしないと、自分自身がクリップボードに送ったテキストにも反応する。
            if (SelectedTab.LastCopiedText == text)
            {
                return;
            }

            var t = text.Trim();
            SelectedTab.TextListItems.Add(new TextListItem(t));
        }

        [Conditional("DEBUG")]
        private void SetDummyData()
        {
            var vm = TextListViewModels.First();
            vm.TextListItems.Add(new TextListItem("Dummy Text 1111") { Info = "lang info1", IsCodeBlock = true, });
            vm.TextListItems.Add(new TextListItem("Dummy Text 2222") { Info = "lang info2", IsCodeBlock = true, });

            vm.TextListItems.Add(new TextListItem("Dummy Text 2222") { IsCodeBlock = false, });

            vm.TextListItems.Add(new TextListItem("Dummy Text 3333") { Info = "lang info3", IsCodeBlock = true, });

            vm.TextListItems.Add(new TextListItem("Dummy Text 4444") { Info = "lang info4", IsCodeBlock = true, IsIncluded = false, });
            vm.TextListItems.Add(new TextListItem("Dummy Text 5555") { Info = "lang info5", IsCodeBlock = false, IsIncluded = false, });
            vm.TextListItems.Add(new TextListItem(new string('a', 200)) { Info = "lang info5", IsCodeBlock = false, IsIncluded = false, });
            vm.TabHeader = "Dummy Tab (default)";
        }
    }
}