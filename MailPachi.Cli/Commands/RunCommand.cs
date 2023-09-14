using System.Diagnostics;
using MailKit;
using MailPachi.Cli.Settings;
using MailPachi.Shared;
using MailPachi.Shared.Service;
using Spectre.Console;
using Spectre.Console.Cli;
// ReSharper disable ConvertToLambdaExpression

namespace MailPachi.Cli.Commands;

public class RunCommand : AsyncCommand<RunSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, RunSettings settings)
    {
        var mailService = new IMapService(settings.Host, settings.Email, settings.Password);
        var appSettings = AppSettings.LoadFormFile(settings.SettingsPath);
        
        var summaries = await AnsiConsole.Status().StartAsync($"[deepskyblue4_1]Since[/]: {settings.Since: yyyy-MM-dd} [deepskyblue4_1]Until[/]: {settings.Until: yyyy-MM-dd} [slateblue3]件名を取得しています...[/]", async _ =>
        { 
            var result = await mailService.GetSummariesAsync((settings.Since.ToUniversalTime(), settings.Until.ToUniversalTime().AddDays(1)), appSettings.Pattern.SenderAddressList, appSettings.Pattern.ExcludeEmails);
            return result.ToList();
        });

        var matchedSummaries = summaries.Where(x => MailParser.IsMatch(x.Envelope.Subject, appSettings.Pattern)).ToList();
        
        var selectPrompt = new SelectionPrompt<UniqueId>().Title("開封する合否メールを選択してください").PageSize(10);
        selectPrompt.Converter = id =>
        {
            var summary = matchedSummaries.First(x => x.UniqueId == id);
            return $"[deepskyblue4_1]Subject[/]: {summary.Envelope.Subject} [deepskyblue4_1]From[/]: {summary.Envelope.From}";
        };
        selectPrompt.AddChoices(matchedSummaries.Select(x => x.UniqueId));
        var selectedUniqueId =  AnsiConsole.Prompt(selectPrompt);
        
        var mail = await AnsiConsole.Status().StartAsync($"[deepskyblue4_1]Since[/]: {settings.Since: yyyy-MM-dd} [deepskyblue4_1]Until[/]: {settings.Until: yyyy-MM-dd} 本文を取得しています...", async _ =>
        {
            return await mailService.GetTextBody(selectedUniqueId);
        });
        var parsedMail = MailParser.ParseBody(mail, appSettings.Pattern);

        if (parsedMail.ResultType == ResultType.Unknown)
        {
            AnsiConsole.MarkupLine("[red]合否判定に失敗しました。[/]");
            return 1;
        }

        var video = appSettings.YoutubeVideos
                                            .Where(x => x.IsPass == (parsedMail.ResultType == ResultType.Pass))
                                            .Select(x => (x, Guid: Guid.NewGuid()))
                                            .MinBy(x => x.Guid).x;

        if (!AnsiConsole.Confirm("合否を表す動画を表示してもよろしいですか？")) return 0;
        
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = video.Url,
            UseShellExecute = true,
            WindowStyle = ProcessWindowStyle.Maximized
        }) ?? throw new Exception();
        await process.WaitForExitAsync();
        return 0;
    }
}