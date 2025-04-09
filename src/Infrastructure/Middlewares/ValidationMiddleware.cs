using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Reflection;
using Microsoft.AspNetCore.Builder;

namespace Finacle.Infrastructure.Middlewares
{

    public class RequestValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Regex ValidStringPattern = new(@"^[a-zA-Z0-9_\s/\-:]*$", RegexOptions.Compiled);

        public RequestValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var errors = new List<string>();

            // ✅ Validate Route Parameters
            foreach (var routeValue in context.Request.RouteValues)
            {
                if (routeValue.Value is string str)
                {
                    ValidateString(str, routeValue.Key, errors);
                }
            }

            // ✅ Validate Body only for applicable methods
            if (context.Request.Method == HttpMethods.Post ||
                context.Request.Method == HttpMethods.Put ||
                context.Request.Method == HttpMethods.Patch)
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrWhiteSpace(body))
                {
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(body);
                        ValidateJson(jsonDoc.RootElement, errors);
                    }
                    catch (JsonException)
                    {
                        errors.Add("Invalid JSON format.");
                    }
                }
            }

            // ❌ Return validation errors if any
            if (errors.Any())
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { errors });
                return;
            }

            await _next(context);
        }

        private void ValidateJson(JsonElement element, List<string> errors, string path = "")
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in element.EnumerateObject())
                {
                    var propPath = string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}";
                    ValidateJson(prop.Value, errors, propPath);
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                int index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    ValidateJson(item, errors, $"{path}[{index}]");
                    index++;
                }
            }
            else if (element.ValueKind == JsonValueKind.String)
            {
                var value = element.GetString();
                if (value != null)
                {
                    ValidateString(value, path, errors);
                }
            }
        }

        private void ValidateString(string value, string path, List<string> errors)
        {
            if (value.Length < 3 || value.Length > 100)
            {
                errors.Add($"'{path}' must be between 3 and 1000 characters.");
            }

            if (!ValidStringPattern.IsMatch(value))
            {
                errors.Add($"'{path}' contains invalid characters. Only letters, numbers, underscores, and spaces are allowed.");
            }
        }
    }
}
