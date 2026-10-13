using System.Collections.ObjectModel;
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

        public TextListViewModel(ToastService toastService)
        {
            this.toastService = toastService;
        }

        public TextListItem? SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public string TabHeader { get => tabHeader; set => SetProperty(ref tabHeader, value); }

        public string LastCopiedText { get; private set; } = string.Empty;

        public ObservableCollection<TextListItem> TextListItems { get; set; } = new ();

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

        public DelegateCommand ClearTextListItemsCommand => new (() =>
        {
            TextListItems.Clear();
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

        private void MoveItem(TextListItem param, int moveIndex)
        {
            var index = TextListItems.IndexOf(param);
            if (index < 0)
            {
                return;
            }

            TextListItems.Move(index, Math.Max(Math.Min(index + moveIndex, TextListItems.Count), 0));
        }
    }
}