using System.Threading.Tasks;

namespace Codeshell.Abp.Integration.Twilio
{
    public interface ITwilioHttpService
    {
        Task<TwilioResultDto> SendMessage(TwilioRequestDto request);
    }
}
