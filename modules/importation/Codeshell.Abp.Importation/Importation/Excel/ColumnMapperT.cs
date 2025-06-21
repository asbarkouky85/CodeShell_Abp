using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Codeshell.Abp.Importation.Excel
{
    public class ColumnMapper<T>
    {
        Dictionary<string, int> _dic = new Dictionary<string, int>();

        public void Map<TValue>(Expression<Func<T, TValue>> ex, int col)
        {
            _dic[((MemberExpression)ex.Body).Member.Name] = col;
        }
        public Dictionary<string, int> Get()
        {
            return _dic;
        }
    }
}
