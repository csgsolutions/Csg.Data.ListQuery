using System;
using Csg.ListQuery.AspNetCore;
using Csg.ListQuery.AspNetCore.ModelBinding;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds default ListQuery services to the given service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddListQuery(this IServiceCollection services, Action<ListRequestOptions> setupAction = null)
    {
        if (setupAction != null)
        {
            services.Configure(setupAction);
        }

        services.TryAddSingleton<IListRequestValidator, DefaultListQueryValidator>();
        services.TryAddSingleton<ListRequestFactory>();

        return services;
    }
}