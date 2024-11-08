using Csg.ListQuery.AspNetCore.OpenApi;
using Csg.ListQuery.Server;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;

namespace AspNetCoreWebAPI.Internal;

public class SwashbuckleValidationHintAttribute : System.Attribute
{
    public SwashbuckleValidationHintAttribute(Type validationType)
    {
        ValidationType = validationType;
    }

    public Type ValidationType { get; set; }

    public int MaxRecursionDepth { get; set; } = 1;
}

public class ListRequestQueryStringOperationFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var parameters = context.MethodInfo.GetParameters();
           
        if (parameters.Length == 1 && typeof(IListRequest).IsAssignableFrom(parameters[0].ParameterType) && context.ApiDescription.HttpMethod == "GET")
        {
            var validationOptions = parameters[0].GetCustomAttribute<SwashbuckleValidationHintAttribute>();

            // set recursive true if you want to include navigation properties, such as WeatherForecast.Author
            operation.Parameters = TypeHelper.GetParametersForHttpGetRequest(parameters[0].ParameterType, validationOptions.ValidationType, maxRecusionDepth: validationOptions.MaxRecursionDepth);
        }
    }
}