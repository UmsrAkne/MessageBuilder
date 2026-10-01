using System.Collections.ObjectModel;

namespace MessageBuilder.ViewModels
{
    public class TextListViewModel
    {
        public ObservableCollection<TextListItem> TextListItems { get; set; } = new ();

        public DelegateCommand AddTextItemCommand => new DelegateCommand(() =>
        {
            TextListItems.Add(new TextListItem(string.Empty) { Info = string.Empty, });
        });

        public DelegateCommand<TextListItem> DeleteTextItemCommand => new (item =>
        {
            TextListItems.Remove(item);
        });

        public DelegateCommand ClearTextListItemsCommand => new (() =>
        {
            TextListItems.Clear();
        });
    }
}