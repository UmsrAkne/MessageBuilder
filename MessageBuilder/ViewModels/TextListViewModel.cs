using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using MessageBuilder.Models;
using MessageBuilder.Services;

namespace MessageBuilder.ViewModels
{
    public class TextListViewModel : BindableBase
    {
        private PromptBuilder promptBuilder = new ();
        private readonly ToastService toastService;
        private AsyncRelayCommand copyToClipboardCommand;
        private TextListItem? selectedItem;
        private string tabHeader;
        private TextListItem? currentFirstItem;

        public TextListViewModel(ToastService toastService)
        {
            TextListItems.CollectionChanged += OnTextListItemsCollectionChanged;
            this.toastService = toastService;
        }

        public TextListItem? SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public string TabHeader { get => tabHeader; set => SetProperty(ref tabHeader, value); }

        public string LastCopiedText { get; private set; } = string.Empty;

        public ObservableCollection<TextListItem> TextListItems { get; set; } = new ();

        public bool HasSaved { get; set; }

        public string SourceFilePath { get; set; } = string.Empty;

        public DelegateCommand AddTextItemCommand => new DelegateCommand(() =>
        {
            TextListItems.Add(new TextListItem(string.Empty) { Info = string.Empty, });
        });

        public DelegateCommand<TextListItem> DeleteTextItemCommand => new (item =>
        {
            TextListItems.Remove(item);
        });

        public DelegateCommand<TextListItem?> MoveUpItemCommand => new ((param) =>
        {
            var target = param ?? SelectedItem;
            if (target == null)
            {
                return;
            }

            MoveItem(target, -1);
            SelectedItem = target;
        });

        public DelegateCommand<TextListItem?> MoveDownItemCommand => new ((param) =>
        {
            var target = param ?? SelectedItem;
            if (target == null)
            {
                return;
            }

            MoveItem(target, 1);
            SelectedItem = target;
        });

        public DelegateCommand<TextListItem> ToggleCodeblockCommand => new ((item) =>
        {
            item.ToggleCodeBlock();
        });

        public DelegateCommand SetCodeBlockCommand => new (() =>
        {
            if (SelectedItem == null)
            {
                return;
            }

            SelectedItem.IsCodeBlock = true;
        });

        public AsyncRelayCommand CopyToClipboardAsyncCommand =>
            copyToClipboardCommand = new AsyncRelayCommand(async () =>
            {
                var result = promptBuilder.BuildPrompt([.. TextListItems,]);
                LastCopiedText = result;
                Clipboard.SetText(result);

                await toastService.ShowToastAsync("Copied to clipboard");
            });

        public TextListDto ExportData()
        {
            return new TextListDto(
                TabHeader: TabHeader,
                Items: TextListItems.Select(item => new TextListItemDto(
                    Text: item.Text,
                    Info: item.Info,
                    IsCodeBlock: item.IsCodeBlock,
                    IsIncluded: item.IsIncluded)).ToList());
        }

        public void ImportData(TextListDto data)
        {
            TabHeader = data.TabHeader;
            TextListItems.Clear();

            foreach (var itemData in data.Items)
            {
                var item = new TextListItem(itemData.Text)
                {
                    IsCodeBlock = itemData.IsCodeBlock,
                    Info = itemData.Info,
                    IsIncluded = itemData.IsIncluded,
                };

                TextListItems.Add(item);
            }
        }

        private void MoveItem(TextListItem param, int moveIndex)
        {
            var index = TextListItems.IndexOf(param);
            if (index < 0)
            {
                return;
            }

            TextListItems.Move(index, Math.Max(Math.Min(index + moveIndex, TextListItems.Count), 0));
        }

        private void OnTextListItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateFirstItemSubscription();
            UpdateTabHeader();
        }

        /// <summary>
        /// 先頭アイテムのテキスト変更イベントの購読を更新します。
        /// </summary>
        private void UpdateFirstItemSubscription()
        {
            var newFirstItem = TextListItems.FirstOrDefault();

            if (ReferenceEquals(currentFirstItem, newFirstItem))
            {
                return;
            }

            if (currentFirstItem != null)
            {
                currentFirstItem.PropertyChanged -= OnFirstItemPropertyChanged;
            }

            currentFirstItem = newFirstItem;

            if (currentFirstItem != null)
            {
                currentFirstItem.PropertyChanged += OnFirstItemPropertyChanged;
            }
        }

        private void OnFirstItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TextListItem.Text))
            {
                UpdateTabHeader();
            }
        }

        /// <summary>
        /// 先頭アイテムのテキストから TabHeader を生成・設定します。
        /// </summary>
        private void UpdateTabHeader()
        {
            var firstItem = TextListItems.FirstOrDefault();
            if (firstItem == null || string.IsNullOrWhiteSpace(firstItem.Text))
            {
                TabHeader = "new Tab";
                return;
            }

            // 改行を取り除き、先頭から文字を取得する。
            var singleLineText = firstItem.Text.Replace("\r", string.Empty).Replace("\n", " ").Trim();
            const int maxLength = 12;

            TabHeader = singleLineText.Length > maxLength
                ? $"{singleLineText[..maxLength]}..."
                : singleLineText;
        }
    }
}