using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Refit;
using Finacle.Infrastructure.Logging;

namespace Transactions.API.Filters
{
    public class ValidateModelFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.ToDictionary(
                    k => k.Key,
                    v => v.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                );

                var response = ResponseManager.ReturnFailedValidations( errors );


                context.Result = new OkObjectResult(response);
            }
        }
    }
}
