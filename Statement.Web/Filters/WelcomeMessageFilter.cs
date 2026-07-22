using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Statement.Web.Filters
{
    public class WelcomeMessageFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as Controller;
            if (controller == null) return;

            var user = context.HttpContext.User;
            var session = context.HttpContext.Session;

            var welcomeMessage = user.FindFirstValue("WelcomeMessage");

            if (string.IsNullOrEmpty(welcomeMessage))
            {
                welcomeMessage = session.GetString("WelcomeMessage");
            }

            if (string.IsNullOrEmpty(welcomeMessage))
            {
                var userName = user.Identity?.Name ?? session.GetString("user_name");
                welcomeMessage = string.IsNullOrEmpty(userName)
                    ? "Welcome to GT Bank Statement Portal!"
                    : $"Welcome back, {userName}!";
                session.SetString("WelcomeMessage", welcomeMessage);
            }

            controller.ViewBag.WelcomeMessage = welcomeMessage;
            controller.ViewBag.UserName = user.Identity?.Name ?? session.GetString("user_name") ?? "User";
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Nothing needed here
        }
    }
}