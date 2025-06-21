using Codeshell.Abp.Exceptions;
using Codeshell.Abp.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.Uow;
using Volo.Abp.Validation;

namespace Codeshell.Abp
{
    public enum CharType { Capital, Small, Both }
    public static class Utils
    {
        static Random r = new Random();
        static int Addition = 0;
        static int currentSecond;

        private static LoggerConfiguration eventLoggerConf;
        public static Serilog.ILogger EventLogger { get; private set; }
        public static void StartEventsLogger(LoggerConfiguration loggerConfiguration)
        {
            eventLoggerConf = loggerConfiguration;
            EventLogger = loggerConfiguration.CreateLogger();
        }

        public static Serilog.ILogger GetEventLogger()
        {
            return eventLoggerConf.CreateLogger();
        }

        public static void CreateFolderForFile(string filePath)
        {
            FileInfo info = new FileInfo(filePath);
            if (!Directory.Exists(info.Directory.FullName))
                Directory.CreateDirectory(info.Directory.FullName);
        }

        public static long GenerateID()
        {
            DateTime t = DateTime.Now;

            int thisSec = (int)t.TimeOfDay.TotalSeconds;

            if (thisSec == currentSecond)
                Addition++;
            else
                Addition = 0;

            currentSecond = thisSec;

            string st = (t.Year - 2000).ToString()
                + t.DayOfYear.ToString("D3")
                + currentSecond.ToString("D5")
                + Addition.ToString("D3");
            return long.Parse(st);
        }

        public static bool TryParseDate(string str, string[] formats, out DateTime t)
        {

            if (!DateTime.TryParse(str, out t))
            {
                if (formats == null)
                    return false;
                return DateTime.TryParseExact(str, formats, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.AdjustToUniversal, out t);
            }
            else
            {
                return true;
            }
        }

        public static bool TryConvertFromHijri(string date, out DateTime dat)
        {

            DateTimeFormatInfo DTFormat = new CultureInfo("ar-sa", false).DateTimeFormat;
            DTFormat.Calendar = new UmAlQuraCalendar();

            DTFormat.ShortDatePattern = "dd/MM/yyyy";


            return DateTime.TryParse(date, DTFormat, DateTimeStyles.None, out dat);
        }

        public static string CombineUrl(params string[] parts)
        {
            var ret = new StringBuilder();
            Regex reg = new Regex("^/");
            bool first = true;
            for (var i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                if (string.IsNullOrEmpty(part))
                    continue;
                part = part.Replace("\\", "/").Trim();
                part = part.Replace(((char)8203).ToString(), string.Empty);
                if (!first)
                {

                    part = reg.Replace(part, "");
                }


                first = false;
                if (i < parts.Length - 1)
                {
                    char last = part[part.Length - 1];
                    part += last != '/' ? "/" : "";
                }
                ret.Append(part);
            }
            return ret.ToString();
        }

        public static AbpValidationException GetException(params ValidationResult[] res)
        {
            var ex = new AbpValidationException(res.First().ErrorMessage);
            foreach (var err in res)
                ex.ValidationErrors.Add(err);
            return ex;
        }

        public static AbpValidationException GetException(string message, params ValidationResult[] res)
        {
            var ex = new AbpValidationException(message);
            foreach (var err in res)
                ex.ValidationErrors.Add(err);
            return ex;
        }

        public static AbpValidationException GetException(IStringLocalizer loc, params ValidationResult[] res)
        {

            var frst = res.First();
            var ex = new AbpValidationException(loc.GetString(frst.ErrorMessage, frst.MemberNames?.Select(e => (object)e).ToArray()));
            foreach (var err in res)
            {
                err.ErrorMessage = loc.GetString(err.ErrorMessage, err.MemberNames?.Select(e => (object)e).ToArray());
                ex.ValidationErrors.Add(err);
            }

            return ex;
        }

        public static AbpValidationException GetException(string message, IStringLocalizer loc, params ValidationResult[] res)
        {
            var ex = new AbpValidationException(message);
            foreach (var err in res)
            {
                err.ErrorMessage = loc.GetString(err.ErrorMessage, err.MemberNames?.Select(e => (object)e).ToArray());
                ex.ValidationErrors.Add(err);
            }

            return ex;
        }


        public static string CamelCaseToWords(string id, string separator)
        {
            Regex r = new Regex("[A-Z]");
            MatchCollection col = r.Matches(id);
            int i = 0;

            foreach (Match d in col)
            {
                if (d.Index != 0)
                {
                    id = id.Insert(d.Index + i++, separator);
                }
            }
            return id;
        }

        public static string RandomNumber(int digits)
        {
            var st = new StringBuilder();
            Random rndm = new Random();

            for (int i = 0; i < digits; i++)
            {
                st.Append(rndm.Next(0, 9));
            }
            return st.ToString();
        }

        public static string RandomAlphaNumeric(int numOfChars, CharType? type = null)
        {
            type = type ?? CharType.Small;
            string allalpha = "012345689ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var len = allalpha.Length;
            var st = new StringBuilder();
            Random rndm = new Random();

            for (int i = 0; i < numOfChars; i++)
            {
                switch (type)
                {
                    case CharType.Capital:
                        st.Append(allalpha[rndm.Next(0, len)].ToString());
                        break;
                    case CharType.Small:
                        st.Append(allalpha[rndm.Next(0, len)].ToString().ToLower());
                        break;
                    case CharType.Both:
                        var n = rndm.Next(0, 2);
                        var c = allalpha[rndm.Next(0, len)].ToString();
                        if (n == 0)
                            c = c.ToLower();
                        st.Append(c);
                        break;
                }
            }
            return st.ToString();
        }

        public static void LogEventPublish(object evnt)
        {
            string data = "";
            try
            {
                data = JsonConvert.SerializeObject(evnt);
            }
            catch { }

            EventLogger.Information("MQ_PUBLISH[" + evnt.GetType().FullName + "]");

        }

        static int _counter = 0;

        public static async Task TraceAction(string identifier, Func<Task> action, bool throwException = true)
        {
            _counter += 1;
            var id = _counter;
            try
            {
                EventLogger.Information($"ACTION[" + identifier + "]");
                await action();
            }
            catch (Exception ex)
            {
                EventLogger.Information("ACTION[" + id + "]");
                EventLogger.Error(ex.GetMessageRecursive());
                if (throwException)
                    throw;
            }
        }

        public static async Task HandleEvent<T>(T evnt, Func<Task> action, bool throwException = true)
        {
            _counter += 1;
            var id = _counter;
            try
            {
                EventLogger.Information("MQ_EVENT[" + id + "] [" + evnt.GetType().FullName + "] [" + evnt + "]");
                await action();
            }
            catch (Exception ex)
            {
                EventLogger.Information("MQ_ERROR[" + id + "][" + evnt.GetType().FullName + "][" + evnt.ToString() + "]");
                EventLogger.Error(ex.GetMessageRecursive());
                if (throwException)
                    throw;
            }
        }

        public static string GetResourceAsString(Assembly assembly, string key)
        {
            string content = null;
            using (var resStream = assembly.GetManifestResourceStream(key))
            {
                using (var reader = new StreamReader(resStream))
                {
                    content = reader.ReadToEnd();
                }
            }
            return content;
        }

        public static string GetDefaultPermission(Type t)
        {
            var refl = t.GetFields().FirstOrDefault(e => e.Name == "Default");
            return refl?.GetValue(null).ToString();
        }

        public static string[] Permissions(Type t, IEnumerable<string> ignore = null)
        {
            var refl = t.GetFields();
            List<string> strings = new List<string>();
            foreach (var f in refl)
            {

                var value = f.GetValue(null).ToString();
                if (ignore == null || !ignore.Any(ign => value.Contains(ign)))
                {
                    strings.Add(value);
                }

            }
            return strings.ToArray();
        }

        public static IConfigurationRoot LoadConfigurationFrom(string path, string environment = null)
        {
            environment = environment ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var conf = new ConfigurationBuilder();
            //BuildConfiguration(conf);
            Console.WriteLine("Reading config from [" + path + "\\appsettings." + (environment == null ? "" : environment) + ".json");
            conf.AddJsonFile(Path.Combine(path, $"appsettings.json"), true, true);
            if (environment != null)
            {
                conf.AddJsonFile(Path.Combine(path, $"appsettings.{environment}.json"), true, true);
            }
            Environment.SetEnvironmentVariable("CODESHELL_SETTINGS_PATH", path);
            return conf.Build();
        }

        public async static Task RunWithUnitOfWork(Func<IServiceProvider, Task> action)
        {
            var logger = NullLogger.Instance;
            logger.LogInformation("Run Scoped Requested");

            using (var sc = CodeshellRoot.RootProvider.CreateScope())
            {
                var uowManager = sc.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

                using (var unit = uowManager.Begin())
                {
                    try
                    {
                        await action(sc.ServiceProvider);
                        await unit.CompleteAsync();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ex.GetMessageRecursive());
                    }
                }
            }
        }

    }
}
