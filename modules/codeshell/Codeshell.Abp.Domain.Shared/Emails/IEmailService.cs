using Codeshell.Abp.Files;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeshell.Abp.Emails
{
    public interface IEmailService
    {
        Task<SendEmailResult> SendEmail(string To, string Subject, string MsgBody, bool html = false, string displayName = "no-Reply", IEnumerable<FileBytes> files = null);
    }
}