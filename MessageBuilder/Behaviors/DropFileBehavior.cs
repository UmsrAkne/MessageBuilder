using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MessageBuilder.Utils;
using MessageBuilder.ViewModels;
using Microsoft.Xaml.Behaviors;

namespace MessageBuilder.Behaviors
{
    public class DropFileBehavior : Behavior<ItemsControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject == null)
            {
                return;
            }

            AssociatedObject.AllowDrop = true;
            AssociatedObject.DragOver += OnDragOver;
            AssociatedObject.Drop += OnDrop;
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.DragOver -= OnDragOver;
                AssociatedObject.Drop -= OnDrop;
            }

            base.OnDetaching();
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            try
            {
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files == null || files.Length == 0)
                {
                    return;
                }

                if (AssociatedObject.ItemsSource is not ObservableCollection<TextListItem> list)
                {
                    return;
                }

                foreach (var file in files)
                {
                    if (string.IsNullOrWhiteSpace(file))
                    {
                        continue;
                    }

                    string text;
                    try
                    {
                        text = System.IO.File.ReadAllText(file);
                    }
                    catch (Exception exception)
                    {
                        AppLogger.Error($"{file} の読み込みに失敗したため、スキップします。", exception);
                        continue;
                    }

                    list.Add(new TextListItem(text)
                    {
                        IsCodeBlock = true,
                        Info = file,
                    });
                }

                e.Handled = true;
            }
            catch
            {
                // ignore unexpected errors to avoid crashing on a drop
                e.Handled = true;
            }
        }
    }
}