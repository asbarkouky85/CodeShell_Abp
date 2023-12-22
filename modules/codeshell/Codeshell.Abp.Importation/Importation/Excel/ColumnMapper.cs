using System;
using System.Collections.Generic;

namespace Codeshell.Abp.Importation.Excel
{
    public class ColumnMapper
    {
        public static Dictionary<string, int> GetMapper<T>(Action<ColumnMapper<T>> action)
        {
            var mapper = new ColumnMapper<T>();
            action.Invoke(mapper);
            return mapper.Get();
        }
    }
}
