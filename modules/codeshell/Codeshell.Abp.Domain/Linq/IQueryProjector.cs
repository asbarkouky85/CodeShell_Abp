using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeshell.Abp.Linq
{
    public interface IQueryProjector
    {
        IQueryable<TDestination> Project<TSource, TDestination>(IQueryable<TSource> query);
    }
}
