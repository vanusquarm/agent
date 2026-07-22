using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Statement.Web.Filters
{
    public class DateFormatSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(DateTime) || context.Type == typeof(DateTime?))
            {
                schema.Example = new Microsoft.OpenApi.Any.OpenApiString("2026-01-31");
                schema.Format = "date";
                schema.Description = "Date in ISO format (yyyy-MM-dd). Example: 2026-01-31";
            }
        }
    }

}
