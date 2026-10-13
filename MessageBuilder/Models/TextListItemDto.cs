namespace MessageBuilder.Models
{
    public record TextListItemDto(
        string Text,
        string Info,
        bool IsCodeBlock,
        bool IsIncluded);
}