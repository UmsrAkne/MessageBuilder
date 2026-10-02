using System.Text;
using MessageBuilder.ViewModels;

namespace MessageBuilder.Services
{
    public class PromptBuilder
    {
        /// <summary>
        /// 有効な TextListItem を Markdown 形式で結合してプロンプト文字列を生成します。
        /// </summary>
        /// <param name="textListItems">プロンプトに含めるアイテムのリスト</param>
        /// <returns>結合されたプロンプト文字列</returns>
        public string BuildPrompt(IEnumerable<TextListItem>? textListItems)
        {
            if (textListItems == null)
            {
                return string.Empty;
            }

            // 1. IsIncluded が true で、テキストが空でないものが対象
            var targetItems = textListItems
                .Where(item => item is { IsIncluded: true, } && !string.IsNullOrWhiteSpace(item.Text));

            var sb = new StringBuilder();

            foreach (var item in targetItems)
            {
                // 各ブロックの間に空行を挟む
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                }

                if (item.IsCodeBlock)
                {
                    // コードブロック形式: ```{Info}\n{Text}\n```
                    var info = item.Info?.Trim() ?? string.Empty;
                    sb.AppendLine($"```{info}");
                    sb.AppendLine(item.Text.TrimEnd());
                    sb.Append("```");
                }
                else
                {
                    // 通常テキスト
                    sb.Append(item.Text.Trim());
                }
            }

            return sb.ToString();
        }
    }
}