using Csg.ListQuery.Server;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace Csg.ListQuery.AspNetCore.ModelBinding;

/// <summary>
/// A model binder that creates <see cref="IListRequest"/> instances from one or more querystring key/value pairs.
/// </summary>
/// <remarks></remarks>
public class ListRequestQueryStringModelBinder : IModelBinder
{
    /// <summary>
    /// Initializes a new instance using the given factory.
    /// </summary>
    /// <param name="factory"></param>
    public ListRequestQueryStringModelBinder(ListRequestFactory factory)
    {
        Factory = factory;
    }

    /// <summary>
    /// Gets the factory instance associated with this binder.
    /// </summary>
    protected ListRequestFactory Factory { get; private set;  }

    /// <summary>
    /// Creates a list request from the given model binding context.
    /// </summary>
    /// <param name="bindingContext"></param>
    /// <returns></returns>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }
            
        if (!bindingContext.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }
            
        var modelName = bindingContext.ModelName;
            
        try
        {
            var listRequest = Factory.CreateRequest(bindingContext.ModelType, bindingContext.HttpContext.Request);

            bindingContext.Result = ModelBindingResult.Success(listRequest);
        }
        catch (NotSupportedException ex)
        {
            bindingContext.ModelState.AddModelError(modelName, ex.Message);
        }    
        catch (FormatException ex)
        {
            bindingContext.ModelState.AddModelError(modelName, ex.Message);
        }

        return System.Threading.Tasks.Task.CompletedTask;
    }
}