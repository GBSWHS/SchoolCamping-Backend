using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolCamping.Models.Responses;

namespace SchoolCamping.Attributes
{
    public class RequireCaptchaAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Items.ContainsKey("captcha"))
            {
                var m = new GeneralResponseModel();
                m.Success = false;
                m.Message = "Make sure you're not a robot";
                context.Result = new JsonResult(m);
                context.HttpContext.Response.StatusCode = 401;
                return;
            }

            if (!(bool)context.HttpContext.Items["captcha"])
            {
                var m = new GeneralResponseModel();
                m.Success = false;
                m.Message = "captcha failed.";
                context.Result = new JsonResult(m);
                context.HttpContext.Response.StatusCode = 401;
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
