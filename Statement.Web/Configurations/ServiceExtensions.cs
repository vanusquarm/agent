using Microsoft.OpenApi.Models;
using Statement.Web.Filters;
using Statement.Web.Interfaces;
using Statement.Web.Services;

namespace Statement.Web.Configurations
{
    public static class ServiceExtensions
    {
        public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ISSOService, SSOService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IMiddlewareClient, MiddlewareClient>();
            builder.Services.AddScoped<IGeneratePdfService, GeneratePdfService>();
            builder.Services.AddScoped<IStatementClient, StatementClient>();
            builder.Services.AddScoped<IGenerateExcelService, GenerateExcelService>();
            builder.Services.AddScoped<IBasisFallbackService, BasisFallbackService>();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "GT Bank Statement API",
                    Version = "v1",
                    Description = @"
                    ## STATEMENT API Documentation

                    ### Date Format
                    All dates should be provided in ISO format: `yyyy-MM-dd`

                    | Example | Description |
                    |---------|-------------|
                    | `2024-01-01` | Start of year |
                    | `2024-12-31` | End of year |
                    | `2024-06-15` | Mid-year |

                    ### Account Number Format
                    Account numbers should follow GT Bank  pattern

                    | Example | Description |
                    |---------|-------------|
                    | `1010012345671` | Valid account format |
                    | `2010012345672` | Valid account format |
                    | `3201001054103` | Valid account format |

                    ### Response Formats
                    - `pdf` - Returns a PDF file download
                    - `excel` - Returns an Excel file download  
                    - `json` - Returns JSON data

                    ### Authentication
                    All API endpoints require an API Key passed in the `X-API-Key` header.

                    ### Email Statements
                    If `email` is not provided, the system will attempt to fetch it from the customer's profile.
                    "
                });

                c.SchemaFilter<DateFormatSchemaFilter>();
                // Define API Key Authentication
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "API Key authentication. Enter your API key in the text input below.",
                    Name = "X-API-Key",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                // Add security requirement
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                {
                    new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    }
                },
                    Array.Empty<string>()
                    }
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });
            return builder;
        }
    }
}