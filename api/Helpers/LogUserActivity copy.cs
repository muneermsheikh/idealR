using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

public class PostSelectionAcceptanceActivity : IAsyncActionFilter
{
    // updates lastActive field of loggedin user.
        public object OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            /* var resultContext = await next();

                var repo = resultContext.HttpContext.RequestServices.GetRequiredService<ISelDecisionRepository>();

                if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
                {
                    var attribute = actionDescriptor.MethodInfo.GetCustomAttribute<MyLogAttribute>();

                    if (attribute != null)
                    {
                        context.HttpContext.Items["MyLogData"] = GetRelevantLogData(context.ActionArguments); // Apply some custom logic to select relevant log data
                    }
                }
            }
            public void OnActionExecuted(ActionExecutedContext context)
            {
                if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
                {
                    var attribute = actionDescriptor.MethodInfo.GetCustomAttribute<MyLogAttribute>();

                    if (attribute != null)
                    {
                        var actionParametersData = (MyActionParametersLogData)context.HttpContext.Items["MyLogData"]

                        LoggerHelper.Log(actionParametersData, context.Result);
                    }
                }
            }
            */

            return null;
        }

    Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        throw new NotImplementedException();
    }
}