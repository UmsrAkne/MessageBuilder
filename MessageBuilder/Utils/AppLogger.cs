using System.IO;
using Serilog;

namespace MessageBuilder.Utils
{
    public static class AppLogger
    {
        static AppLogger()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var logPath = Path.Combine(baseDir, "logs", "app-log.txt");

            // Debug レベル以上のログをコンソールとファイルに出力する
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day, // 日次ローテーション設定
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        // 呼び出し用のメソッドたちや
        public static void Debug(string message)
        {
            Log.Debug(message);
        }

        public static void Info(string message)
        {
            Log.Information(message);
        }

        public static void Warn(string message)
        {
            Log.Warning(message);
        }

        public static void Error(string message, Exception ex)
        {
            Log.Error(ex, message);
        }

        // プログラム終了時に確実にログを吐き出すための口や
        public static void Shutdown()
        {
            Log.CloseAndFlush();
        }
    }
}