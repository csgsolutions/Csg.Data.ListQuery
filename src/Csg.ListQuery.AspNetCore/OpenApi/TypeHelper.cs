using Csg.ListQuery.Server;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace Csg.ListQuery.AspNetCore.OpenApi;

/// <summary>
/// Provides helper methods
/// </summary>
public static class TypeHelper
{
    /// <summary>
    /// Gets the list of Open API parameters for the given request type.
    /// </summary>
    /// <param name="requestType"></param>
    /// <param name="validationType"></param>
    /// <returns></returns>
    public static IList<Microsoft.OpenApi.Models.OpenApiParameter> GetParametersForHttpGetRequest(Type requestType, Type validationType, int? maxRecusionDepth)
    {
        var result = new List<Microsoft.OpenApi.Models.OpenApiParameter>();
        var fields = Csg.ListQuery.AspNetCore.PropertyHelper.GetProperties(validationType, maxRecursionDepth: maxRecusionDepth);
        bool sortable = false;

        result.Add(new OpenApiParameter()
        {
            Name = "fields",
            In = ParameterLocation.Query,
            Style = ParameterStyle.Simple,
            Schema = new OpenApiSchema() { Type = "string" },
            Description = "A comma separated list of fields to include."
        });

        if (typeof(IListRequest).IsAssignableFrom(requestType))
        {
            result.Add(new OpenApiParameter()
            {
                Name = "offset",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema() { Type = "integer", Format = "int32" },
                Description = "The zero based index of the first record to return."
            });

            result.Add(new OpenApiParameter()
            {
                Name = "limit",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema() { Type = "integer", Format = "int32", Minimum = 0 },
                Description = "The total number of rows to return in a single request."
            });
        }

        foreach (var field in fields)
        {
            if (field.Value.IsFilterable == true)
            {
                result.Add(new OpenApiParameter()
                {
                    Name = $"where[{field.Key}]",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema() { Type = "string" },
                    Description = field.Value.Description ?? "A filter value with an optional operator prefix."
                });
            }

            if (field.Value.IsSortable == true)
            {
                sortable = true;
            }
        }

        if (sortable)
        {
            result.Add(new OpenApiParameter()
            {
                Name = "order",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema() { Type = "string" },
                Description = "A field to sort the response query by. Prefix with a dash to sort descending.",
            });
        }

        return result;
    }
}