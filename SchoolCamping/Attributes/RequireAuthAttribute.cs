﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolCamping.Models.Responses;

namespace SchoolCamping.Attributes
{
    public class RequireAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Items.ContainsKey("admin"))
            {
                var m = new GeneralResponseModel();
                m.Success = false;
                m.Message = "https://www.youtube.com/watch?v=MWql1CBYlk8";
                context.Result = new JsonResult(m);
                context.HttpContext.Response.StatusCode = 401;
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
