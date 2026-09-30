using System.Collections.ObjectModel;
using System.Diagnostics;
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
            SetDummyData();
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public ObservableCollection<TextListItem> TextListItems { get; set; } = new ();

        public DelegateCommand AddTextItemCommand => new DelegateCommand(() =>
        {
            TextListItems.Add(new TextListItem(string.Empty) { Info = string.Empty, });
        });

        public DelegateCommand<TextListItem> DeleteTextItemCommand => new (item =>
        {
            TextListItems.Remove(item);
        });

        [Conditional("DEBUG")]
        private void SetDummyData()
        {
            TextListItems.Add(new TextListItem("Dummy Text 1111") { Info = "lang info1", IsCodeBlock = true, });
            TextListItems.Add(new TextListItem("Dummy Text 2222") { Info = "lang info2", IsCodeBlock = true, });

            TextListItems.Add(new TextListItem("Dummy Text 2222") { IsCodeBlock = false, });

            TextListItems.Add(new TextListItem("Dummy Text 3333") { Info = "lang info3", IsCodeBlock = true, });
        }
    }
}