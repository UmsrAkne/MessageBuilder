namespace MessageBuilder.Utils
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Windows;

    public static class ClipboardHelper
    {
        private const int DefaultRetryCount = 5;
        private const int DefaultRetryDelayMilliseconds = 100;

        /// <summary>
        /// 指定したテキストをリトライ付きでクリップボードに設定します。
        /// </summary>
        /// <param name="text">コピーするテキスト</param>
        /// <param name="retryCount">最大試行回数</param>
        /// <param name="retryDelayMilliseconds">再試行までの待機時間（ミリ秒）</param>
        /// <returns>コピーに成功した場合は true、失敗した場合は false</returns>
        public static async Task<bool> TrySetTextAsync(string text, int retryCount = DefaultRetryCount, int retryDelayMilliseconds = DefaultRetryDelayMilliseconds)
        {
            for (var i = 0; i < retryCount; i++)
            {
                try
                {
                    // copy: true により、アプリケーション終了後もクリップボードにデータを残す
                    Clipboard.SetDataObject(text, true);
                    return true;
                }
                catch (ExternalException)
                {
                    // 他のプロセスがクリップボードを開いている場合に発生
                    if (i == retryCount - 1)
                    {
                        return false;
                    }

                    await Task.Delay(retryDelayMilliseconds);
                }
                catch (Exception)
                {
                    // 想定外の例外はリトライせず即座に失敗とする
                    return false;
                }
            }

            return false;
        }
    }
}