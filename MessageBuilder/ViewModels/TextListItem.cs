namespace MessageBuilder.ViewModels
{
    public class TextListItem : BindableBase
    {
        private string text = string.Empty;
        private string info = string.Empty;
        private bool isCodeBlock;

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
    }
}