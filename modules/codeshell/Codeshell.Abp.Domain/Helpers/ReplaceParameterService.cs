using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Volo.Abp.Domain.Services;

namespace Codeshell.Abp.Helpers
{
    public class ReplaceParameterService : DomainService, IReplaceParameterService
    {
        public string ReplaceParametersUsingFile(string filePath, object parameters)
        {
            if (!File.Exists(filePath))
                throw new Exception("File not found : " + filePath);
            PropertyInfo[] props = parameters.GetType().GetProperties();
            string contents = File.ReadAllText(filePath);
            foreach (PropertyInfo inf in props)
            {
                contents = contents.Replace("%" + inf.Name + "%", inf.GetValue(parameters)?.ToString());
            }
            return contents;
        }

        public string ReplaceParameters(string contents, object parameters)
        {
            PropertyInfo[] props = parameters.GetType().GetProperties();
            foreach (PropertyInfo inf in props)
            {
                contents = contents.Replace("%" + inf.Name + "%", inf.GetValue(parameters)?.ToString());
            }
            return contents;
        }
    }
}
