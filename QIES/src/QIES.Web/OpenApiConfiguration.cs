using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using QIES.Common.Records;

namespace QIES.Web
{
    public static class OpenApiConfiguration
    {
        public static IServiceCollection AddOpenApi(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.MapType<ServiceNumber>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "servicenumber",
                    Pattern = @"^[1-9]\d{4}$",
                    Example = new OpenApiString("11110")
                });
                c.MapType<ServiceName>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "servicename",
                    Pattern = @"^[a-zA-Z0-9'][a-zA-Z0-9' ]+[a-zA-Z0-9']$",
                    Example = new OpenApiString("A Service Name")
                });
                c.MapType<ServiceDate>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "servicedate",
                    Pattern = @"^[12]\d{3}[01][0-2][0-3]\d$",
                    Example = new OpenApiString("20181207")
                });
                c.MapType<NumberTickets>(() => new OpenApiSchema {
                    Type = "integer",
                    Minimum = 1,
                    Maximum = 1000
                });

                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "QIES.Web.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "QIES.Api.xml"));
            });

            return services;
        }

        public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app)
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "openapi/{documentName}/openapi.json";
            });
            app.UseReDoc(c =>
            {
                c.RoutePrefix = "openapi";
                c.SpecUrl = "v1/openapi.json";
                c.ExpandResponses("");
            });

            return app;
        }
    }
}
