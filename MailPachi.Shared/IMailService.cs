using MailKit;

namespace MailPachi.Shared.Service;

public abstract class MailServiceBase
{
    public abstract Task GetSummariesAsync((DateTime since, DateTime until) dateFilter, IEnumerable<string>? senderFilter = null, IEnumerable<string>? excludeSenderFilter = null);
    public abstract Task<IEnumerable<Mail>> GetTextBodies(IEnumerable<UniqueId> ids);
    public abstract Task<Mail> GetTextBody(UniqueId id);
}