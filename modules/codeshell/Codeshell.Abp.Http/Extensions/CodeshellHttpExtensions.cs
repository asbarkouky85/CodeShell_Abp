using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.VirtualFileSystem;
using Codeshell.Abp.Extensions.Json;
using Microsoft.AspNetCore.SignalR;
using Codeshell.Abp.Emitters;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Runtime.ConstrainedExecution;

namespace Codeshell.Abp.Extensions
{

    public static class CodeshellHttpExtensions
    {
        public static void AddSignalRHub<TContract, THub>(this IServiceCollection coll) where THub : Hub<TContract> where TContract : class
        {
            coll.AddTransient<IEmitter<TContract>, SignalREmitter<THub, TContract>>();
        }


        public static bool IsCodeshellDevelopment(this IHostEnvironment env)
        {
            var envs = new[] { "local", "local-db", "test-server", "test-server-auth" };
            return env.IsDevelopment() || envs.Contains(env.EnvironmentName);
        }

        public static void SeedIfRequired(this ApplicationInitializationContext context)
        {
            if (context.GetConfiguration().SeedOnStartup())
            {
                AsyncHelper.RunSync(async () =>
                {
                    using (var scope = context.ServiceProvider.CreateScope())
                    {
                        await scope.ServiceProvider.SeedData();
                    }
                });
            }
        }



        public static IWebHostBuilder UseKestrelHttps(this ConfigureWebHostBuilder builder)
        {
            var lSettings = "Properties/launchSettings.json";
            int httpsPort = 5001;
            int httpPort = 5000;
            string sslCert = "Root,localhost";
            string sslPassword = null;
            if (File.Exists(lSettings))
            {
                var data = File.ReadAllText(lSettings);
                var obj = (JObject?)JsonConvert.DeserializeObject(data);
                var httpUrl = obj.GetPathAsString("iisSettings:iisExpress:applicationUrl");
                var httpPortString = httpUrl?.GetAfterLast(":")?.Replace("/", "");
                var httpsPortString = obj.GetPathAsString("iisSettings:iisExpress:sslPort");
                var sslCertString = obj.GetPathAsString("iisSettings:iisExpress:sslCertificate");
                var sslPassString = obj.GetPathAsString("iisSettings:iisExpress:sslPassword");
                if (!string.IsNullOrEmpty(httpPortString))
                    int.TryParse(httpPortString, out httpPort);
                if (!string.IsNullOrEmpty(httpsPortString))
                    int.TryParse(httpsPortString, out httpsPort);
                if (!string.IsNullOrEmpty(sslCertString))
                    sslCert = sslCertString;
                if (!string.IsNullOrEmpty(sslPassString))
                    sslPassword = sslPassString;

                httpsPort = httpsPort == 0 ? 5001 : httpsPort;
            }

            builder.ConfigureKestrel(op =>
            {
                op.Listen(IPAddress.Any, httpsPort, lop =>
                {
                    lop.Protocols = HttpProtocols.Http1AndHttp2;

                    if (sslCert.Contains(","))
                    {
                        var store = StoreName.Root;
                        var crtName = "localhost";
                        var spl = sslCert.Split(",");
                        if (spl.Length > 1)
                        {
                            store = Enum.Parse<StoreName>(spl[0], true);
                            crtName = spl[1];

                        }
                        var st = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                        st.Open(OpenFlags.ReadOnly);
                        var list = st.Certificates.Find(X509FindType.FindBySubjectName, crtName, false);
                        if (!list.Any())
                        {
                            list = st.Certificates.Find(X509FindType.FindBySerialNumber, crtName, false);
                        }

                        if (list.Any())
                        {
                            var cert = list.First();
                            lop.UseHttps(cert);
                        }
                    }
                    else
                    {
                        var cert = new X509Certificate2(sslCert, sslPassword, X509KeyStorageFlags.MachineKeySet);
                        lop.UseHttps(cert);
                    }

                });
                if (httpPort != httpsPort)
                {
                    op.Listen(IPAddress.Any, httpPort);
                }
            });
            return builder;
        }

        public static void ConfigureManehCors(this ServiceConfigurationContext context, IConfiguration configuration, string policyName)
        {
            context.Services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder =>
                {
                    builder
                        .WithOrigins(
                            configuration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .WithAbpExposedHeaders()
                        .WithExposedHeaders("Content-Disposition")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        public static void UseCodeshellSwaggerUI<T>(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseSwagger();
            var apiName = typeof(T).Assembly.GetName().Name?.Replace(".Host", "");
            var title = apiName.GetAfterLast(".");
            app.UseAbpSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", apiName + " API");
                options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
                options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
                options.DocumentTitle = title;
            });
        }

        public static void ConfigureManehSwagger<T>(this ServiceConfigurationContext context, IConfiguration configuration)
        {
            var apiName = typeof(T).Assembly.GetName().Name?.Replace(".Host", "");
            var title = apiName.GetAfterLast(".");
            context.Services.AddAbpSwaggerGenWithOAuth(
                configuration["AuthServer:Authority"],
                new Dictionary<string, string>
                {
                    { title, apiName??"" }
                },
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = title + " API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(e => e.FullName);
                });
        }

        public static void ConfigureGatewaySwagger<T>(this ServiceConfigurationContext context, IConfiguration configuration, params Type[] modules)
        {
            var dic = new Dictionary<string, string>();
            var assemblyName = typeof(T).Assembly.GetName().Name?.Replace(".Host", "");
            var title = assemblyName.GetAfterLast(".");
            foreach (var m in modules)
            {
                var val = typeof(T).Assembly.GetName().Name?.Replace(".HttpApi", "");
                var key = val.GetAfterLast(".");
                dic[key] = val;
            }

            context.Services.AddAbpSwaggerGenWithOAuth(
                configuration["AuthServer:Authority"],
                dic,
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = title + " API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(e => e.FullName);
                });
        }

        public static bool SeedOnStartup(this IConfiguration context)
        {
            return context.GetValue("SeedOnStartup", false);
        }

        public static void ConfigureManehAuthentication(this ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, e =>
                {
                    e.Authority = configuration["AuthServer:Authority"];
                    e.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                    e.TokenValidationParameters.RequireAudience = false;
                    e.TokenValidationParameters.ValidateAudience = false;
                });
        }

        public static void AddLocalizationPhysicalFiles<T>(this AbpVirtualFileSystemOptions options, string root, string moduleName)
        {

            var path = $"..\\..\\modules\\{moduleName}\\src\\{moduleName}.Domain.Shared";
            if (root.Contains("\\bin\\"))
            {
                path = "..\\..\\..\\" + path;
            }

            options.FileSets.ReplaceEmbeddedByPhysical<T>(
                Path.Combine(root, path));
        }

        public static bool TryReadHeader<T>(this HttpRequest req, string header, out T value)
        {
            if (req.Headers.TryGetValue(header, out StringValues vals))
            {
                var str = vals.First();
                TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));
                var cov = typeConverter.ConvertFromString(str);

                if (cov != null)
                {
                    value = (T)cov;
                    return true;
                }

            }
            value = default;
            return false;
        }

        public static ActionResult GetAssemblyInfo(this Controller cont)
        {
            string? assemblyName = cont.GetType().Assembly.GetName().Name;
            string ver = cont.GetType().Assembly.GetVersionString();
            cont.Response.ContentType = "text/html";
            return cont.Content("<head><title>" + assemblyName + "</title></head><h1>" + assemblyName + "</h1><h2>Version : " + ver + "</h2>");
        }

    }

}
