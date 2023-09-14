namespace MailPachi.Shared;

public class ParseResult
{
    public ParseResult(ResultType resultType, string subject, string? textContent)
    {
        ResultType = resultType;
        Subject = subject;
        TextContent = textContent;
    }

    public ResultType ResultType { get; }
    public string Subject { get; }
    public string? TextContent { set; get; }
}