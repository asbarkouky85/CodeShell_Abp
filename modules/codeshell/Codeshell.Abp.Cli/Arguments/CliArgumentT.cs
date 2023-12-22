using Codeshell.Abp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Codeshell.Abp.Cli.Arguments
{
    public class CliArgument<T, TVal> : CliArgument<T> where TVal : class where T : class
    {
        protected Expression<Func<T, TVal>> _action;
        private bool _isBool;
        public override bool IsBool => _isBool;
        public TVal? DefautValue { get; private set; }
        private bool _defaultIsSet = false;

        public CliArgument(Expression<Func<T, TVal>> t, string? key = null, string? shortKey = null, int? order = null, bool required = false) : base()
        {
            var memberExpression = t.Body as MemberExpression;
            if (shortKey != null && shortKey.Length > 2)
            {
                throw new Exception("shortKey should not be longer than 2 characters");
            }
            CharacterSymbol = shortKey;
            _action = t;
            Key = key;
            Order = order;
            IsRequired = required;

            if (memberExpression != null)
            {
                MemberName = key ?? memberExpression.Member.Name;
                Description = Utils.CamelCaseToWords(memberExpression.Member.Name, " ");
            }
            _isBool = typeof(TVal).RealType() == typeof(bool);
        }

        public CliArgument<T, TVal> SetDefault(TVal def)
        {
            _defaultIsSet = true;
            DefautValue = def;
            return this;
        }

        public CliArgument<T, TVal> SetOptions(string[] options)
        {
            Options = options;
            return this;
        }

        public override void SetMemberValue(T obj, string v)
        {
            var b = _action.Body as MemberExpression;

            if (obj != null && b != null)
            {
                var mem = obj.GetType().GetProperty(b.Member.Name);
                var val = _getValue(v);
                mem?.SetValue(obj, val);
            }
            IsSet = true;
        }

        private TVal? _getValue(string value)
        {
            if (typeof(TVal).IsEnum)
            {
                if (int.TryParse(value, out int res))
                {
                    return (TVal)Enum.ToObject(typeof(TVal), res);
                }
                else if (Enum.TryParse(typeof(TVal), value, out object? resEnum))
                {
                    return (TVal?)resEnum;
                }
            }
            return value.ConvertTo<TVal>();
        }

        public override string? GetDefault()
        {
            return _defaultIsSet ? (DefautValue?.ToString()) : null;
        }
    }
}
