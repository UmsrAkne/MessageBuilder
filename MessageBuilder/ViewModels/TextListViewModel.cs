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

        public DelegateCommand<TextListItem> MoveUpItemCommand => new ((param) =>
        {
            MoveItem(param, -1);
        });

        public DelegateCommand<TextListItem> MoveDownItemCommand => new ((param) =>
        {
            MoveItem(param, 1);
        });

        public DelegateCommand ClearTextListItemsCommand => new (() =>
        {
            TextListItems.Clear();
        });

        private void MoveItem(TextListItem param, int moveIndex)
        {
            var index = TextListItems.IndexOf(param);
            if (index < 0)
            {
                return;
            }

            TextListItems.RemoveAt(index);
            TextListItems.Insert(Math.Max(Math.Min(index + moveIndex, TextListItems.Count), 0), param);
        }
    }
}