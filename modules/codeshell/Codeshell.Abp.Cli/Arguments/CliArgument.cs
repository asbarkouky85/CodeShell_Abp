using Codeshell.Abp.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Codeshell.Abp.Cli.Arguments
{
    public abstract class CliArgument<T> where T : class
    {
        public string? CharacterSymbol { get; protected set; }
        public string? Key { get; protected set; }
        public int? Order { get; protected set; }
        public bool IsRequired { get; protected set; }
        public bool IsSet { get; protected set; }
        public string MemberName { get; protected set; }
        public virtual bool IsBool { get; }
        public string Description { get; set; }
        public string[]? Options { get; protected set; }

        //public T Item;

        protected CliArgument()
        {
            IsRequired = false;
            IsSet = false;
            IsSet = false;
            Description = string.Empty;
            MemberName = string.Empty;
        }

        public abstract void SetMemberValue(T obj, string v);

        public virtual string? GetDefault()
        {
            return null;
        }
    }

}
