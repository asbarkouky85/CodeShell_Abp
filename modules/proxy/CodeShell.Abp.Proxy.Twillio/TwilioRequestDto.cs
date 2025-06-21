using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Codeshell.Abp.Text;

namespace Codeshell.Abp.Integration.Twilio
{
    public class TwilioRequestDto
    {
        public string To { get; set; }
        public string From { get; set; }
        public string ContentSid { get; set; }
        public Dictionary<string,string> ContentVariables { get; set; }

    }
}
