using System.IO;
using MessageBuilder.ViewModels;

namespace MessageBuilder.Services
{
    public class TabTemplateService
    {
        private readonly string templateFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "default_tab_template.json");

        private readonly TextListSerializer textListSerializer = new ();

        // テンプレートが存在するか確認
        public bool HasTemplate()
        {
            return File.Exists(templateFilePath);
        }

        // テンプレートから ViewModel を復元（既存のデシリアライズ処理を利用）
        public void LoadTemplate(TextListViewModel targetVm)
        {
            if (File.Exists(templateFilePath))
            {
                textListSerializer.DeserializeInto(templateFilePath, targetVm);
            }
        }

        // 現在の状態をテンプレートとして保存
        public void SaveAsTemplate(TextListViewModel sourceVm)
        {
            textListSerializer.SerializeSingle(sourceVm, templateFilePath);
        }

        public void RemoveTemplate()
        {
            File.Delete(templateFilePath);
        }
    }
}