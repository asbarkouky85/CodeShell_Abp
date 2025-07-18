﻿using Codeshell.Abp.Cli;
using Codeshell.Abp.Text;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.CliDispatch.Controllers
{
    public abstract class ConsoleController
    {
        private int? last;
        protected ConsoleService writer;
        protected IServiceProvider Injector { get { return _scope.ServiceProvider; } }
        public bool IsMain { get; set; }
        protected string CurrentMethod { get; set; }
        protected virtual void OnMethodSelected(string name) { }
        private IServiceScope _scope;
        private IAbpLazyServiceProvider _lazyServiceProvider;
        public abstract Dictionary<int, string> Functions { get; }

        public ConsoleController()
        {
            var output = CodeshellRoot.RootProvider.GetService<IOutputWriter>();
            writer = new ConsoleService(output);
        }

        protected IServiceProvider GetProvider()
        {
            return _scope.ServiceProvider;
        }

        public async Task Run()
        {
            await StartRouting("Function", Functions);
        }

        public T WaitForTask<T>(Task<T> task)
        {
            Task.WaitAll(task);
            return task.Result;
        }

        protected T GetService<T>()
        {
            return _lazyServiceProvider.LazyGetRequiredService<T>();
        }

        public string GetStringFromUser(string v)
        {
            Console.Write(v + " : ");
            return Console.ReadLine();
        }

        public bool GetBoolFromUser(string v, bool? def = null)
        {
            while (true)
            {
                Console.WriteLine();
                string defaultString = def == null ? "" : def.Value ? "[y]" : "[n]";
                Console.Write(v + $"? (y/n) {defaultString}: ");

                var st = Console.ReadLine();
                Console.WriteLine();
                if (st.ToLower() == "n")
                    return false;
                else if (st.ToLower() == "y")
                    return true;
                else if (def != null)
                    return def.Value;
                Console.WriteLine("Invalid Choice");
            }
        }

        protected string GetSelectionFromUser(string v, string[] s)
        {
            Dictionary<int, string> mods = new Dictionary<int, string>();
            for (int i = 0; i < s.Length; i++)
                mods[i + 1] = s[i];
            return GetSelectionFromUser(v, mods);
        }

        protected string GetSelectionFromUser(string v, Dictionary<int, string> data, bool nameToPhrase = false)
        {
            Console.WriteLine();
            var choices = GenerateChoices(data, nameToPhrase);

            while (true)
            {
                int function = 0;
                Console.Write(string.Format(@"
Select a {0} from the List

    {1}

Enter Your Choice (0: Exit) : ", v, choices));

                string l = Console.ReadLine();
                if (!int.TryParse(l, out function) || function > data.Count || function < 1 && function != 0)
                {
                    Console.WriteLine("Invalid Choice");
                    continue;
                }

                if (function == 0)
                {
                    break;
                }
                return data[function];
            }
            return null;
        }

        protected async Task StartRouting(string name, Dictionary<int, string> functions, bool nameToPhrase = true)
        {
            int function = 0;
            bool restart = false;
            while (true)
            {
                string data = GenerateChoices(functions, nameToPhrase);
                Console.Write(string.Format(@"
Select a {0} from the List

    {1}

Enter Your Choice (0: Exit{2}) : ", name, data, IsMain ? " | r: Restart" : ""));

                string l = Console.ReadLine();
                if (IsMain && l.ToLower() == "r")
                {
                    restart = true;
                    break;
                }
                if (!int.TryParse(l, out function) || function > Functions.Count || function < 1 && function != 0)
                {
                    Console.WriteLine("Invalid Choice");
                    continue;
                }

                if (function == 0)
                {
                    break;
                }


                MethodInfo info = GetType().GetMethod(Functions[function]);

                OnMethodSelected(Functions[function]);
                CurrentMethod = Functions[function];


                try
                {

                    using (var sc = CodeshellRoot.RootProvider.CreateScope())
                    {
                        _scope = sc;
                        _lazyServiceProvider = sc.ServiceProvider.GetRequiredService<IAbpLazyServiceProvider>();
                        if (info.ReturnType.IsAssignableFrom(typeof(Task)))
                        {
                            await (Task)info.Invoke(this, new object[0]);
                        }
                        else
                        {
                            await Task.Run(() =>
                            {
                                info.Invoke(this, new object[] { });
                            });
                        }

                    }
                }
                catch (Exception ex)
                {
                    writer.WriteException(ex);
                }
            }
            if (restart)
            {
                Console.WriteLine();
                Console.WriteLine("Restarting...");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "run",
                    UseShellExecute = true,
                    CreateNoWindow = false
                });

            }

        }

        protected int GetIntFromUser(string message, int min = 1, int max = 0)
        {
            while (true)
            {
                string def = last.HasValue ? $"[{last.Value}]" : "";
                Console.Write(message + " (0 to exit function) " + def + ": ");
                int termId = 0;
                string l = Console.ReadLine();
                if (string.IsNullOrEmpty(l) && last != null)
                    return last.Value;

                if (!int.TryParse(l, out termId) || termId > max && max != 0 || termId < min && termId != 0)
                {
                    Console.WriteLine("Invalid Choice");
                    continue;
                }
                last = termId;
                return termId;
            }

        }

        protected string GenerateChoices(Dictionary<int, string> choices, bool nameToPhrase = false)
        {
            string st = "";
            foreach (var c in choices)
            {
                var name = nameToPhrase ? LangUtils.IdToPhrase(c.Value) : c.Value;
                st += string.Format("\t{0}. {1}\n", c.Key, name);
            }
            return st;
        }
    }
}
