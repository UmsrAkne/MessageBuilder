using System.Windows;

namespace MessageBuilder.Services
{
    public interface IClipboardWatchService
    {
        event EventHandler<string>? TextCopied;

        void Start(Window window);

        void Stop();
    }
}