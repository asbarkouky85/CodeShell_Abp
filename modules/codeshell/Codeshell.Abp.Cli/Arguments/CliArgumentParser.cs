using System;
using System.Linq;

namespace Codeshell.Abp.Cli.Arguments
{
    public class CliArgumentParser<T> where T : class
    {
        public ICliRequestBuilder<T> Builder { get; private set; }
        public CliArgumentParser()
        {
            Builder = new CliRequestBuilder<T>();
        }
        public virtual T Parse(string[] args)
        {
            var requestInstance = Activator.CreateInstance<T>();
            CliArgument<T>? valueFor = null;

            for (var i = 1; i < args.Length; i++)
            {
                if (valueFor != null)
                {
                    valueFor.SetMemberValue(requestInstance, args[i]);
                    valueFor = null;
                }
                else if (TryGetArgumentItem(args[i], out CliArgument<T>? item))
                {
                    if (item != null && item.IsBool)
                    {
                        item.SetMemberValue(requestInstance, "True");
                    }
                    else
                    {
                        valueFor = item;
                    }

                }
                else if (TryGetArgumentItem(i, out CliArgument<T>? itemByOrder))
                {
                    itemByOrder?.SetMemberValue(requestInstance, args[i]);
                }
            }

            if (Builder.IsInvalid(out string[] lst))
            {
                throw new Exception(lst[0]);
            }

            Builder.UseDefaultsForUnset(requestInstance);
            return requestInstance;
        }

        protected virtual bool TryGetArgumentItem(int order, out CliArgument<T>? argItem)
        {
            argItem = Builder.KeyList.Where(e => e.Order == order).FirstOrDefault();
            return argItem != null;
        }

        protected virtual bool TryGetArgumentItem(string arg, out CliArgument<T>? argItem)
        {
            argItem = null;
            if (!arg.StartsWith("-"))
                return false;

            if (arg.StartsWith("--"))
            {
                var key = arg.Replace("--", "");
                argItem = Builder.KeyList.FirstOrDefault(e => e.Key == key);
            }
            else
            {
                var key = arg.Replace("-", "");
                argItem = Builder.KeyList.FirstOrDefault(e => e.CharacterSymbol == key);
            }

            return argItem != null;
        }
    }
}
