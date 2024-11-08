using Csg.ListQuery.Server;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;

namespace Csg.ListQuery.AspNetCore.ModelBinding;

public class ListRequestModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (typeof(IListRequest).IsAssignableFrom(context.Metadata.ModelType))
        {
            return new BinderTypeModelBinder(typeof(ListRequestQueryStringModelBinder));
        }

        return null;
    }
}