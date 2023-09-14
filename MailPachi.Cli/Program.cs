using System.Text;
using MailPachi.Cli.Commands;
using Spectre.Console.Cli;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); //　.NET Coreで日本語用の変な文字コードように必要(shift_jisとか)
        
        var commandApp = new CommandApp();
        commandApp.Configure(x =>
        {
            x.AddCommand<RunCommand>("run");
        });
        return await commandApp.RunAsync(args);
    }
}