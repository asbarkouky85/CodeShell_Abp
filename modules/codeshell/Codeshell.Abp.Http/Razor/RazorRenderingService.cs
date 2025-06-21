using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System.Net;
using Volo.Abp.Application.Services;

namespace Codeshell.Abp.Razor
{
    public class RazorRenderingService : IRazorRenderingService
    {
        protected IRazorViewEngine _razorViewEngine;
        protected ITempDataProvider _tempDataProvider;
        public RazorRenderingService(IRazorViewEngine engine, ITempDataProvider tmp)
        {
            _razorViewEngine = engine;
            _tempDataProvider = tmp;
        }

        public async Task<string> RenderViewAsync(HttpContext context, string viewName, object model = null, Dictionary<string, object> viewData = null)
        {
            ActionContext actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
            using (var sw = new StringWriter())
            {

                ViewEngineResult viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

                if (viewResult.View == null)
                    throw new Exception($"{viewName} does not match any available view");

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

                if (model != null)
                    viewDictionary.Model = model;

                if (viewData != null)
                {
                    foreach (var item in viewData)
                        viewDictionary[item.Key] = item.Value;
                }

                var opts = new HtmlHelperOptions();

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    opts
                );

                await viewResult.View.RenderAsync(viewContext);

                return sw.ToString();

            }
        }

        public async Task<string> RenderPageAsync(HttpContext context, string pageName, string layout, object model = null, Dictionary<string, object> viewData = null)
        {
            ActionContext actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                RazorPageResult viewResult = _razorViewEngine.FindPage(actionContext, pageName);
                if (viewResult.Page == null)
                    throw new Exception($"{pageName} does not match any available view");

                viewResult.Page.Layout = layout;
                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

                if (model != null)
                    viewDictionary.Model = model;

                if (viewData != null)
                {
                    foreach (var item in viewData)
                        viewDictionary[item.Key] = item.Value;
                }

                var opts = new HtmlHelperOptions();

                var viewContext = new ViewContext();
                //var viewContext = new ViewContext(
                //    actionContext,
                //    null,
                //    viewDictionary,
                //    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                //    sw,
                //    opts
                //);


                viewResult.Page.ViewContext = new ViewContext();
                await viewResult.Page.ViewContext.View.RenderAsync(viewContext);
                await viewResult.Page.ExecuteAsync();
                viewResult.Page.ViewContext.ViewData = viewDictionary;
                viewResult.Page.ViewContext.Writer = sw;

                await viewResult.Page.ViewContext.View.RenderAsync(viewContext);

                return sw.ToString();

            }
        }
    }
}
