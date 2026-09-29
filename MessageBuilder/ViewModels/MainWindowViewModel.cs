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

        [Conditional("DEBUG")]
        private void SetDummyData()
        {
            TextListItems.Add(new TextListItem("Dummy Text 1111") { Info = "lang info1", });
            TextListItems.Add(new TextListItem("Dummy Text 2222") { Info = "lang info2", });
            TextListItems.Add(new TextListItem("Dummy Text 3333") { Info = "lang info3", });
        }
    }
}