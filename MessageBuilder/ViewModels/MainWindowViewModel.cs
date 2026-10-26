using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using MessageBuilder.Services;
using MessageBuilder.Utils;

namespace MessageBuilder.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private readonly IClipboardWatchService clipboardWatchService;
        private readonly Func<TextListViewModel>? textListViewModelFactory;
        private readonly TextListSerializer textListSerializer;
        private string title = "MessageBuilder";
        private bool isClipboardWatching;
        private TextListViewModel? selectedTab;
        private AsyncRelayCommand? stashAndCopyCommand;

        public MainWindowViewModel()
        {
            TextListViewModels.Add(new TextListViewModel(new ToastService()));
            clipboardWatchService = new ClipboardWatcherService();
            textListViewModelFactory = null;
            textListSerializer = new TextListSerializer();
            AppLogger.Info("MainWindowViewModel created (default)");
            SetDummyData();
        }

        public MainWindowViewModel(Func<TextListViewModel> textListViewModelFactory, ToastService toastService, IClipboardWatchService clipboardWatchService)
        {
            ToastService = toastService;
            this.textListViewModelFactory = textListViewModelFactory;
            var defaultVm = textListViewModelFactory();
            defaultVm.TabHeader = "Default";
            TextListViewModels.Add(defaultVm);
            SelectedTab = defaultVm;

            AppLogger.Info("MainWindowViewModel created (DI)");
            this.clipboardWatchService = clipboardWatchService;
            this.clipboardWatchService.TextCopied += OnTextCopied;

            textListSerializer = new TextListSerializer();

            SetDummyData();
        }

        public bool IsDebugMode { get; set; }

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

        public DelegateCommand StashCurrentTabCommand => new (() =>
        {
            if (SelectedTab == null)
            {
                return;
            }

            if (!SelectedTab.HasSaved)
            {
                var savedPaths = textListSerializer.Serialize(new List<TextListViewModel>() { SelectedTab, });
                SelectedTab.HasSaved = true;
                SelectedTab.SourceFilePath = savedPaths.First();
            }

            var newTab = CreateNewTab($"Tab {TextListViewModels.Count:d2}");
            TextListViewModels.Insert(0, newTab);
            SelectedTab = newTab;
        });

        public AsyncRelayCommand StashAndCopyAsyncCommand =>
            stashAndCopyCommand ??= new AsyncRelayCommand(async () =>
            {
                if (SelectedTab == null)
                {
                    return;
                }

                if (SelectedTab.TextListItems.All(t => string.IsNullOrWhiteSpace(t.Text)))
                {
                    await ToastService.ShowToastAsync(ToastService.NoTextToCopyMessage);
                    return;
                }

                await SelectedTab.CopyToClipboardAsyncCommand.ExecuteAsync(null);
                StashCurrentTabCommand.Execute();
            });

        public DelegateCommand<TextListViewModel> RemoveTextListItemCommand => new ((item) =>
        {
            TextListViewModels.Remove(item);
        });

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

        private TextListViewModel CreateNewTab(string tabHeader)
        {
            var newVm = textListViewModelFactory();
            newVm.TabHeader = tabHeader;

            newVm.TextListItems.Add(new TextListItem(string.Empty));
            newVm.TextListItems.Add(new TextListItem(string.Empty) { IsCodeBlock = true, });

            return newVm;
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

            IsDebugMode = true;
        }
    }
}