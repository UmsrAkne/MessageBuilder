using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using MessageBuilder.Utils;

namespace MessageBuilder.Services
{
    public sealed class ClipboardWatcherService : IClipboardWatchService, IDisposable
    {
        private const int WM_CLIPBOARDUPDATE = 0x031D;
        private HwndSource? hwndSource;
        private IntPtr windowHandle;
        private bool isListening;
        private bool disposed;

        public event EventHandler<string>? TextCopied;

        public void Start(Window window)
        {
            if (isListening)
            {
                return;
            }

            var helper = new WindowInteropHelper(window);
            windowHandle = helper.Handle;

            if (windowHandle == IntPtr.Zero)
            {
                // Windowロード前などでハンドルがまだない場合は SourceInitialized でフック
                window.SourceInitialized += OnSourceInitialized;
                return;
            }

            AttachHook();
        }

        public void Stop()
        {
            if (!isListening)
            {
                return;
            }

            NativeMethods.RemoveClipboardFormatListener(windowHandle);
            hwndSource?.RemoveHook(WndProc);
            isListening = false;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            Stop();

            hwndSource?.Dispose();
            hwndSource = null;

            disposed = true;
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            if (sender is Window window)
            {
                window.SourceInitialized -= OnSourceInitialized;
                windowHandle = new WindowInteropHelper(window).Handle;
                AttachHook();
            }
        }

        private void AttachHook()
        {
            hwndSource = HwndSource.FromHwnd(windowHandle);
            hwndSource?.AddHook(WndProc);
            NativeMethods.AddClipboardFormatListener(windowHandle);
            isListening = true;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_CLIPBOARDUPDATE)
            {
                OnClipboardUpdate();
            }

            return IntPtr.Zero;
        }

        private async void OnClipboardUpdate()
        {
            try
            {
                // 他のプロセスがクリップボードを開いていると取得に失敗する場合があるため、数回リトライする
                string? text = null;
                for (var i = 0; i < 5; i++)
                {
                    try
                    {
                        if (Clipboard.ContainsText())
                        {
                            text = Clipboard.GetText();
                            break;
                        }
                    }
                    catch (COMException)
                    {
                        await Task.Delay(50);
                    }
                }

                if (!string.IsNullOrEmpty(text))
                {
                    TextCopied?.Invoke(this, text);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("クリップボードの更新処理中にエラーが発生しました。", ex);
            }
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool AddClipboardFormatListener(IntPtr hwnd);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
        }
    }
}