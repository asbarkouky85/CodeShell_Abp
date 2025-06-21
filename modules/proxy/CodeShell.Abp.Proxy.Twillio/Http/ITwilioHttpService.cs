using System.Threading.Tasks;
using CodeShellCore.Integration.Twilio.Http;

namespace CodeShellCore.Integration.Twilio.Http
{
    public interface ITwilioHttpService
    {
        Task<TwilioResultDto> SendMessage(TwilioRequestDto request);
    }
}
