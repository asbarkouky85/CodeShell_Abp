using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Codeshell.Abp.Cli.Arguments
{
    public interface ICliRequestBuilder<T> where T : class
    {
        IEnumerable<CliArgument<T>> KeyList { get; }
        void Document();
        bool IsInvalid(out string[] res);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TVal"></typeparam>
        /// <param name="t"></param>
        /// <param name="key"></param>
        /// <param name="shortKey">Maximum 2 characters</param>
        /// <param name="order"></param>
        /// <param name="isRequired"></param>
        /// <returns></returns>
        CliArgument<T, TVal> Property<TVal>(Expression<Func<T, TVal>> t, string key, string? shortKey = null, int? order = null, bool isRequired = false) where TVal : class;
        void UseDefaultsForUnset(T subj);
    }
}