namespace MessageBuilder.ViewModels
{
    public class TextListItem : BindableBase
    {
        private string text = string.Empty;
        private string info = string.Empty;
        private bool isCodeBlock;
        private bool isIncluded = true;
        private string previousInfo = string.Empty;

        public TextListItem(string text)
        {
            Text = text;
        }

        public string Text { get => text; set => SetProperty(ref text, value); }

        // コードブロックのトリプルクォートの後ろに書く文字列。言語名などを記述する。
        public string Info
        {
            get => info;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    IsCodeBlock = true;
                }

                SetProperty(ref info, value);
            }
        }

        public bool IsCodeBlock { get => isCodeBlock; set => SetProperty(ref isCodeBlock, value); }

        public bool IsIncluded { get => isIncluded; set => SetProperty(ref isIncluded, value); }

        public void ToggleCodeBlock()
        {
            if (IsCodeBlock)
            {
                // 通常のテキストに Info は不要なので復元できる状態で退避。
                previousInfo = Info;
                Info = string.Empty;
                IsCodeBlock = false;
            }
            else
            {
                IsCodeBlock = true;

                if (!string.IsNullOrEmpty(previousInfo))
                {
                    Info = previousInfo;
                }
            }
        }
    }
}