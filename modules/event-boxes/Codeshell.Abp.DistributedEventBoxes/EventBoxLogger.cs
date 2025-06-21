using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Codeshell.Abp.DistributedEventBoxes;

namespace Codeshell.Abp.DistributedEventBoxes
{
    public class EventBoxLogger : IEventBoxLogger
    {
        public bool Disable { get; set; }
        protected string LoggerId;
        protected bool ShowLogs { get; set; }
        public EventBoxLogger(IOptions<CodeshellEventInboxOptions> opts)
        {
            ShowLogs = opts.Value.ShowConsoleLogs;
        }

        public IEventBoxLogger SetId(string id)
        {
            LoggerId = id == null ? "" : $"[{id}]";
            return this;
        }

        public void Log(string log, byte[] data = null)
        {
            if ((Debugger.IsAttached || ShowLogs) && !Disable)
            {
                if (data != null)
                {
                    var dataString = Encoding.ASCII.GetString(data);
                    log += " " + dataString;
                }
                Console.WriteLine($"[{DateTime.Now.ToString("hh:mm:ss ff")}]{LoggerId}\t" + log);
            }
        }

        public void LogObject(string log, object data)
        {
            if (Debugger.IsAttached || ShowLogs)
            {
                if (data != null)
                {
                    var dataString = JsonSerializer.Serialize(data);
                    log += " " + dataString;
                }
                Log(log);
            }
        }

    }
}
