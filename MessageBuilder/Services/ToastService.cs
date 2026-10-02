namespace MessageBuilder.Services
{
    public sealed class ToastService : BindableBase, IDisposable
    {
        private bool isToastVisible;
        private string toastMessage = string.Empty;
        private CancellationTokenSource? toastCts;
        private bool disposed;

        public string ToastMessage
        {
            get => toastMessage;
            set => SetProperty(ref toastMessage, value);
        }

        public bool IsToastVisible
        {
            get => isToastVisible;
            set => SetProperty(ref isToastVisible, value);
        }

        public async Task ShowToastAsync(string message, int durationMilliseconds = 1500)
        {
            // 前回の表示タイマーが動いていればキャンセル
            toastCts?.Cancel();
            toastCts?.Dispose();
            toastCts = new CancellationTokenSource();
            var token = toastCts.Token;

            ToastMessage = message;
            IsToastVisible = true;

            try
            {
                await Task.Delay(durationMilliseconds, token);
                IsToastVisible = false;
            }
            catch (TaskCanceledException)
            {
                // 新しい通知で上書きされた場合は無視
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                toastCts?.Dispose();
            }

            disposed = true;
        }
    }
}