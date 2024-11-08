using System;
using System.Threading.Tasks;
using Csg.ListQuery.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Csg.ListQuery.AspNetCore.Mvc;

/// <summary>
/// Provides extension methods for handling list requests.
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Handles a list request using the given query execution method.
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TValidation"></typeparam>
    /// <typeparam name="TInfrastructure"></typeparam>
    /// <param name="controller"></param>
    /// <param name="request"></param>
    /// <param name="executeQueryMethod"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    public static async Task<ActionResult<ListResponse<TDomain>>> HandleListRequestAsync<TDomain, TValidation, TInfrastructure>(
        this ControllerBase controller,
        IListRequest request,
        Func<ListQueryDefinition, Task<ListQueryResult<TInfrastructure>>> executeQueryMethod,
        Func<TInfrastructure, TDomain> map
    ) where TDomain : new()
    {
        var validator = controller.HttpContext.RequestServices.GetRequiredService<IListRequestValidator>();
        var domainProperties = validator.GetProperties(typeof(TDomain));
        var validationResult = validator.Validate<TValidation>(request);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                controller.ModelState.AddModelError(error.Field, error.Error);
            }
            return controller.ValidationProblem(controller.ModelState);
        }

        var queryResult = await executeQueryMethod(validationResult.ListQuery).ConfigureAwait(false);

        return queryResult.ToListResponse<TInfrastructure, TDomain>(
            request,
            domainProperties,
            map,
            controller.HttpContext.Request.GetUri()
        );
    }

    public static Task<ActionResult<ListResponse<TDomain>>> HandleListRequestAsync<TDomain, TInfrastructure>(
        this ControllerBase controller,
        IListRequest request,
        Func<ListQueryDefinition, Task<ListQueryResult<TInfrastructure>>> executeQueryMethod,
        Func<TInfrastructure, TDomain> map
    ) where TDomain : new()
    {
        return HandleListRequestAsync<TDomain, TDomain, TInfrastructure>(controller, request, executeQueryMethod, map);
    }

    public static Task<ActionResult<ListResponse<TDomain>>> HandleListRequestAsync<TDomain>(
        this ControllerBase controller,
        IListRequest request,
        Func<ListQueryDefinition, Task<ListQueryResult<TDomain>>> executeQueryMethod
    ) where TDomain : new()
    {
        return HandleListRequestAsync<TDomain, TDomain, TDomain>(controller, request, executeQueryMethod, x => x);
    }
}