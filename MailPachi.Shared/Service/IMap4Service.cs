using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit.Text;

namespace MailPachi.Shared.Service;

public class IMapService : MailServiceBase
{
    private readonly string _hostname;
    private readonly string _emailAddress;
    private readonly string _password;
    private readonly ImapClient _imapClient;

    public IMapService(string hostname, string emailAddress, string password)
    {
        _hostname = hostname;
        _emailAddress = emailAddress;
        _password = password;
        _imapClient = new ImapClient();
    }

    private async Task ConnectAsync()
    {
        await _imapClient.ConnectAsync(_hostname, 993, true);
        await _imapClient.AuthenticateAsync(_emailAddress, _password);
    }

    public override async Task<IEnumerable<IMessageSummary>> GetSummariesAsync((DateTime since, DateTime until) dateFilter, IEnumerable<string>? senderFilter = null, IEnumerable<string>? excludeSenderFilter = null)
    {
        if (_imapClient.IsAuthenticated is false)
        {
            await ConnectAsync();
        }
        
        var inbox = _imapClient.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadWrite);

        var searchQuery = SearchQuery.SentSince(dateFilter.since.ToUniversalTime()).And(SearchQuery.SentBefore(dateFilter.until.ToUniversalTime()));
        if (senderFilter != null)
        {
            searchQuery = senderFilter.Aggregate(searchQuery, (current, filter) => current.And(SearchQuery.FromContains(filter)));
        }
        if (excludeSenderFilter != null)
        {
            searchQuery = excludeSenderFilter.Aggregate(searchQuery, (current, filter) => current.And(SearchQuery.Not(SearchQuery.FromContains(filter))));
        }

        var ids = await inbox.SearchAsync(searchQuery);
        return await inbox.FetchAsync(ids, MessageSummaryItems.Envelope);
    }
    
    public override async Task<IEnumerable<Mail>> GetTextBodies(IEnumerable<UniqueId> ids)
    {
        if (_imapClient.IsAuthenticated is false)
        {
            await ConnectAsync();
        }
        
        var inbox = _imapClient.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadWrite);
        
        return ids.Select(x =>
        {
            var message = inbox.GetMessage(x);
            return new Mail(message.Subject, message.GetTextBody(TextFormat.Text));
        });
    }
    
    public override async Task<Mail> GetTextBody(UniqueId uniqueId)
    {
        if (_imapClient.IsAuthenticated is false)
        {
            await ConnectAsync();
        }
        
        var inbox = _imapClient.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadWrite);
        
        var message = await inbox.GetMessageAsync(uniqueId);
        return new Mail(message.Subject, message.GetTextBody(TextFormat.Text));
    }
}