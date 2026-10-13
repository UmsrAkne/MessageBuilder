using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using MessageBuilder.Models;
using MessageBuilder.ViewModels;

namespace MessageBuilder.Services
{
    public class TextListSerializer
    {
        private readonly static JsonSerializerOptions JsonOptions = new ()
        {
            WriteIndented = true,

            // 日本語などの文字化け（エスケープ）を防ぐ設定
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };

        private readonly string baseDirectory;

        public TextListSerializer(string? baseDirectory = null)
        {
            this.baseDirectory = baseDirectory ?? Path.Combine(AppContext.BaseDirectory, "text_list");
        }

        public void Serialize(IEnumerable<TextListViewModel> viewModels)
        {
            Directory.CreateDirectory(baseDirectory); // フォルダがなければ作る

            var dt = DateTime.Now.ToString("yyyyMMddHHmmss");
            var count = 1;

            foreach (var vm in viewModels)
            {
                var data = vm.ExportData();
                var json = JsonSerializer.Serialize(data, JsonOptions);
                var path = Path.Combine(baseDirectory, $"text{dt}_{count++}.json");
                File.WriteAllText(path, json);
            }
        }

        public void DeserializeInto(string filePath, TextListViewModel targetViewModel)
        {
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<TextListDto>(json, JsonOptions);
            if (data != null)
            {
                targetViewModel.ImportData(data);
            }
        }
    }
}