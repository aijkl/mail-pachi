using System.ComponentModel;
using Spectre.Console.Cli;

namespace MailPachi.Cli.Settings;

public class RunSettings : CommandSettings
{
    public RunSettings(string host, string email, string password, string settingsPath)
    {
        Host = host;
        Email = email;
        Password = password;
        SettingsPath = settingsPath;
    }

    [CommandOption("--host <HOST>")]
    public string Host { get; }
    
    [CommandOption("--email")]
    public string Email { get; }
    
    [CommandOption("--password")]
    public string Password { get; }

    [CommandOption("--settings-path")]
    [DefaultValue("./Appsettings.json")]
    public string SettingsPath { get; }
    
    [CommandOption("--since")]
    public DateTime Since { get; } = DateTime.Now.AddDays(-1);
    
    [CommandOption("--until")]
    public DateTime Until { get; } = DateTime.Now;
}