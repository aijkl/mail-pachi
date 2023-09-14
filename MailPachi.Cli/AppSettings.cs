using MailPachi.Shared;
using Newtonsoft.Json;

namespace MailPachi.Cli;

public class AppSettings
{
    public AppSettings(Pattern pattern)
    {
        Pattern = pattern;
    }

    public Pattern Pattern { get; }

    public static AppSettings LoadFormFile(string path)
    {
        return JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(path)) ?? throw new NullReferenceException();
    }
}