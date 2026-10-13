namespace MessageBuilder.Models
{
    public record TextListDto(
        string TabHeader,
        IReadOnlyList<TextListItemDto> Items);
}