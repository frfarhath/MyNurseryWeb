using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

public static class ViewRenderExtensions
{
    public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false)
    {
        var services = controller.HttpContext.RequestServices;
        var viewEngine = (ICompositeViewEngine)services.GetService(typeof(ICompositeViewEngine));
        var tempDataProvider = (ITempDataProvider)services.GetService(typeof(ITempDataProvider));

        controller.ViewData.Model = model;

        using var sw = new StringWriter();
        var actionContext = new ActionContext(controller.HttpContext, controller.RouteData, controller.ControllerContext.ActionDescriptor);
        var viewResult = viewEngine.FindView(actionContext, viewName, !partial);

        if (viewResult.View == null)
        {
            throw new FileNotFoundException("View cannot be found.");
        }

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            controller.ViewData,
            controller.TempData,
            sw,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
    }
}
