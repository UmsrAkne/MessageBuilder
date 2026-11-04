using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MessageBuilder.ViewModels;

namespace MessageBuilder.Behaviors
{
    public static class ListBoxItemPasteBehavior
    {
        public readonly static DependencyProperty PasteCommandProperty =
            DependencyProperty.RegisterAttached(
                "PasteCommand",
                typeof(ICommand),
                typeof(ListBoxItemPasteBehavior),
                new PropertyMetadata(null, OnPasteCommandChanged));

        public static ICommand? GetPasteCommand(DependencyObject obj)
        {
            return (ICommand?)obj.GetValue(PasteCommandProperty);
        }

        public static void SetPasteCommand(DependencyObject obj, ICommand? value)
        {
            obj.SetValue(PasteCommandProperty, value);
        }

        private static void OnPasteCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListBoxItem item)
            {
                return;
            }

            // 既存の CommandBinding を整理
            for (var i = item.CommandBindings.Count - 1; i >= 0; i--)
            {
                if (item.CommandBindings[i].Command == ApplicationCommands.Paste)
                {
                    item.CommandBindings.RemoveAt(i);
                }
            }

            if (e.NewValue is ICommand)
            {
                var binding = new CommandBinding(
                    ApplicationCommands.Paste,
                    OnExecuted,
                    OnCanExecute);

                item.CommandBindings.Add(binding);
            }
        }

        private static void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                var command = GetPasteCommand(item);
                if (command != null)
                {
                    // クリップボードにテキストがあるかも条件に含める場合はここでチェック
                    if (item.DataContext is not TextListItem dt)
                    {
                        return;
                    }

                    e.CanExecute = Clipboard.ContainsText() && command.CanExecute(dt.Text);
                    e.Handled = true;
                    return;
                }
            }

            e.CanExecute = false;
        }

        private static void OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                var command = GetPasteCommand(item);
                if (command != null && Clipboard.ContainsText())
                {
                    var text = Clipboard.GetText();
                    if (command.CanExecute(text))
                    {
                        command.Execute(text);
                        e.Handled = true;
                    }
                }
            }
        }
    }
}