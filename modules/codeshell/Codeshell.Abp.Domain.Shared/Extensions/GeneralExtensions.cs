using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Codeshell.Abp.Extensions;
using Microsoft.Extensions.Options;
using Volo.Abp.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Codeshell.Abp.Extensions
{
    public static class GeneralExtensions
    {
        

        // public static void ApplyChanges<TEntity,TDto>(this IRepository<TEntity> repo,IEnumerable<TDto> dtos,)
        public static async Task SeedData(this IServiceProvider provider, Guid? tenantId = null, ILogger logger = null)
        {
            var seedContributorTypes = provider.GetRequiredService<IOptions<AbpDataSeedOptions>>().Value
                                    .Contributors;

            foreach (var seedContributorType in seedContributorTypes)
            {
                logger?.LogInformation("\t- Using " + seedContributorType.FullName);
                var seedContributor = (IDataSeedContributor)provider.GetRequiredService(seedContributorType);
                await seedContributor.SeedAsync(new DataSeedContext(tenantId));
            }
        }
        public static string GetVersionString(this Assembly assembly)
        {
            var asem = assembly.CustomAttributes.FirstOrDefault(d => d.AttributeType == typeof(AssemblyFileVersionAttribute));
            if (asem == null)
                return null;
            if (asem.ConstructorArguments.Count == 0)
                return null;
            return (string)asem.ConstructorArguments.FirstOrDefault().Value;
        }

        public static string GetManehModuleName(this Assembly assembly)
        {
            return assembly.GetName().Name.Split(".")[1];
        }

        public static string GetMessageRecursive(this Exception ex)
        {
            string mes = ex.Message;
            if (ex.Data.Contains("Message"))
            {
                mes += " [RemoteServiceError] -> " + ex.Data["Message"];
            }
            if (ex.InnerException != null)
            {
                mes += " > " + ex.InnerException.GetMessageRecursive();
            }
            return mes;
        }

        public static List<T> MapTo<T>(this IEnumerable lst, bool ignoreId = true, IEnumerable<string> ignore = null) where T : class
        {
            List<T> t = new List<T>();
            foreach (var ob in lst)
            {
                t.Add(ob.MapPropertiesTo<T>(ignoreId, ignore));
            }
            return t;
        }

        public static T MapPropertiesTo<T>(this object obj, bool ignoreId = true, IEnumerable<string> ignore = null) where T : class
        {
            T inst = Activator.CreateInstance<T>();
            inst.AppendProperties(obj, ignoreId, ignore);
            return inst;
        }
        public static void AppendProperties(this object model, IDictionary<string, string> ob, IEnumerable<string> ignore = null)
        {
            ignore = ignore ?? new List<string>();

            Dictionary<string, PropertyInfo> modelProps = model.GetType()
                .GetProperties()
                .Where(d =>
                    //(d.PropertyType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(d.PropertyType)) &&
                    !ignore.Contains(d.Name) &&
                    d.CanWrite
                ).ToDictionary(d => d.Name);

            foreach (string key in ob.Keys)
            {
                if (modelProps.ContainsKey(key))
                {
                    modelProps[key].SetValue(model, ob[key].ConvertTo(modelProps[key].PropertyType));
                }
            }
        }
        public static void AppendProperties(this object model, object ob, bool ignoreId = true, IEnumerable<string> ignore = null)
        {
            ignore = ignore ?? new List<string>();
            PropertyInfo[] props = ob.GetType().GetProperties();
            Dictionary<string, PropertyInfo> modelProps = model.GetType()
                .GetProperties()
                .Where(
                    d => (d.Name != "Id" || !ignoreId) &&
                    (d.PropertyType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(d.PropertyType)) &&
                    !ignore.Contains(d.Name) &&
                    d.CanWrite
                )
                .ToDictionary(d => d.Name);

            foreach (PropertyInfo inf in props)
            {
                if (modelProps.ContainsKey(inf.Name))
                {
                    object v = inf.GetValue(ob);
                    modelProps[inf.Name].SetValue(model, v);
                }
            }
        }

        public static Type RealType(this Type type)
        {
            if (type.IsNullable())
                return type.GetGenericArguments()[0];
            return type;
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public static bool IsNullable(this Type type)
        {
            if (type.IsGenericType)
                return type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
            else if (type == typeof(string))
                return true;
            return false;
        }

        public static bool IsDouble(this Type type, bool includeNullable = false)
        {
            if (includeNullable)
                type = type.RealType();
            return type.Equals(typeof(double));
        }

        public static bool IsDecimalType(this Type type, bool includeNullable = false)
        {
            if (includeNullable)
                type = type.RealType();
            return type.Equals(typeof(decimal)) || type.Equals(typeof(double)) || type.Equals(typeof(float));
        }

        public static bool IsManehDevelopmentEnvironment(this string name)
        {
            var envs = new[] { "Development", "local", "local-db", "test-server", "test-server-auth" };
            return envs.Contains(name);
        }

        public static bool IsIntgerType(this Type type, bool includeNullable = false)
        {
            if (includeNullable)
                type = type.RealType();
            return type.Equals(typeof(sbyte)) || type.Equals(typeof(byte)) || type.Equals(typeof(int)) || type.Equals(typeof(long)) || type.Equals(typeof(uint)) || type.Equals(typeof(ulong));
        }

        public static DateTime GetDayStart(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        public static DateTime GetDayEnd(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        }

        public static DateTime RemoveMilli(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        }

        public static string GetString(this Enum @enum)
        {
            return @enum.GetType().Name + "_" + @enum.ToString();
        }

        public static string StringFormat(this Enum @enum)
        {
            return @enum.GetType().Name + "_" + @enum.ToString();
        }

        public static string ToEnumString<T>(this long value)
        {
            var t = typeof(T);
            var x = Enum.GetName(t, value);
            return t.Name + "_" + x;
        }

        public static string ToEnumString<T>(this int value)
        {
            var t = typeof(T);
            var x = Enum.GetName(t, value);
            return t.Name + "_" + x;
        }

        public static IHostBuilder ConfigureUsingAppsettings(this IHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{env}.json", optional: true);
            });
            return builder;
        }

    }
}
