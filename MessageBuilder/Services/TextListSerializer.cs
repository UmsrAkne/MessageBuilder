using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
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

        public void Serialize(IEnumerable<TextListViewModel> viewModels)
        {
            var dt = DateTime.Now.ToString("yyyyMMddHHmmss");
            var count = 1;
            var pathBase = Path.Combine(AppContext.BaseDirectory, "text_list");

            foreach (var vm in viewModels)
            {
                var data = vm.ExportData();
                var json = JsonSerializer.Serialize(data, JsonOptions);
                var path = Path.Combine(pathBase, $"text{dt}_{count++}.json");
                File.WriteAllText(path, json);
            }
        }
    }
}