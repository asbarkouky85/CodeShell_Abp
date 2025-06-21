using Codeshell.Abp.Http;
using Codeshell.Abp.Text;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Codeshell.Abp.Integration.Twilio
{
    public class TwilioHttpService : HttpService, ITwilioHttpService
    {
        protected TwilioOptions Options { get; }

        public TwilioHttpService(IOptions<TwilioOptions> options)
        {
            BaseUrl = options.Value.Url;
            Options = options.Value;
        }
        protected override Task BuildClientAsync(HttpClient client)
        {
            var byteArray = Encoding.ASCII.GetBytes($"{Options.UserName}:{Options.Password}");

            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            return Task.CompletedTask;
        }

        public async Task<TwilioResultDto> SendMessage(TwilioRequestDto request)
        {
            return await PostUrlEncoded<TwilioResultDto>("", request);
        }


    }
}
