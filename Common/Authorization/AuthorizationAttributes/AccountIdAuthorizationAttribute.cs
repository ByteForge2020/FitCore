// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Filters;
//
// namespace Common.Authorization.AuthorizationAttributes;
// [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
// public class AccountIdAuthorizationAttribute : Attribute, IActionFilter
// {
//     public void OnActionExecuting(ActionExecutingContext context)
//     {
//         var authorizationService = context.HttpContext.RequestServices.GetService(typeof(IAuthorization)) as IAuthorization;
//
//         if (authorizationService == null)
//         {
//             context.Result = new UnauthorizedResult();
//             return;
//         }
//
//         try
//         {
//             var accountId = authorizationService.ThrowOrGetAccountId();
//             context.HttpContext.Items["AccountId"] = accountId;
//         }
//         // catch (InvalidOperationException ex)
//         {
//             context.Result = new BadRequestObjectResult(new { Error = ex.Message });
//         }
//     }
//
//     public void OnActionExecuted(ActionExecutedContext context)
//     {
//
//     }
// }