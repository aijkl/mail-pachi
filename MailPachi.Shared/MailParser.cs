using System.Text.RegularExpressions;
// ReSharper disable ConvertIfStatementToReturnStatement

namespace MailPachi.Shared;

public class Pattern
{
    public Pattern(Regex[] failWords, Regex[] passWords, Regex[] subjectWords, string[] excludeEmails, string[] senderAddressList)
    {
        FailWords = failWords;
        PassWords = passWords;
        SubjectWords = subjectWords;
        ExcludeEmails = excludeEmails;
        SenderAddressList = senderAddressList;
    }

    public Regex[] FailWords { get; }
    public Regex[] PassWords { get; }
    public Regex[] SubjectWords { get; }
    public string[] ExcludeEmails { get; }
    public string[] SenderAddressList { get; }
}


public static class MailParser
{
    public static bool IsMatch(string? subject, Pattern pattern)
    {
        if (subject == null) return false;
        
        return pattern.SubjectWords.Any(x => x.IsMatch(subject));
    }
    public static ParseResult ParseBody(Mail mail, Pattern pattern)
    {
        if (pattern.PassWords.Any(x => x.IsMatch(mail.TextContent)))
        {
            return new ParseResult(ResultType.Pass, mail.Subject, mail.TextContent);
        }
        if (pattern.FailWords.Any(x => x.IsMatch(mail.TextContent)))
        {
            return new ParseResult(ResultType.Fail, mail.Subject, mail.TextContent);
        }
        return new ParseResult(ResultType.Unknown, mail.Subject, mail.TextContent);
    }
}