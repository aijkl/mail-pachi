using MailPachi.Shared;
using Newtonsoft.Json;

namespace MailPachi.Cli;

public class AppSettings
{
    public AppSettings(Pattern pattern, IList<YoutubeVideo> youtubeVideos)
    {
        Pattern = pattern;
        YoutubeVideos = youtubeVideos;
    }

    public Pattern Pattern { get; }
    
    public IList<YoutubeVideo> YoutubeVideos { get; set; }

    public static AppSettings LoadFormFile(string path)
    {
        return JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(path)) ?? throw new NullReferenceException();
    }
}