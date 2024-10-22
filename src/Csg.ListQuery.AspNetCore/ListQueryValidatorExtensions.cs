using Csg.ListQuery.Server;

namespace Csg.ListQuery.AspNetCore;

/// <summary>
/// Provides extension methods for list validator.
/// </summary>
public static class ListQueryValidatorExtensions
{
    /// <summary>
    /// Transforms a list request into a repository query
    /// </summary>
    /// <typeparam name="TValidationModel">A type that defines the selections, filters, and sortable fields allowed.</typeparam>
    /// <param name="validator"></param>
    /// <returns></returns>
    public static ListRequestValidationResult Validate<TValidationModel>(this IListRequestValidator validator, IListRequest request, int? maxRecursionDepth = null)
    {
        var properties = validator.GetProperties(typeof(TValidationModel), maxRecursionDepth: maxRecursionDepth);

        return validator.Validate(request, properties, properties, properties);
    }

    /// <summary>
    /// Transforms a list request into a repository query
    /// </summary>
    /// <typeparam name="TFieldValidationModel">A type that defines the selections and sortable fields allowed.</typeparam>
    /// <typeparam name="TFilterValidationModel">A type that defines the filters allowed.</typeparam>
    /// <param name="validator"></param>
    /// <returns></returns>
    public static ListRequestValidationResult Validate<TFieldValidationModel, TFilterValidationModel>(this IListRequestValidator validator, IListRequest request, int? maxRecursionDepth = null)
    {
        var fieldProperties = validator.GetProperties(typeof(TFieldValidationModel), maxRecursionDepth: maxRecursionDepth);
        var filterPropreties = validator.GetProperties(typeof(TFilterValidationModel), maxRecursionDepth: maxRecursionDepth);
        return validator.Validate(request, fieldProperties, filterPropreties, fieldProperties);
    }

    /// <summary>
    /// Transforms a list request into a repository query
    /// </summary>
    /// <typeparam name="TSelectableProperties">A type that defines the selections and sortable fields allowed.</typeparam>
    /// <typeparam name="TFilterableProperties">A type that defines the filters allowed.</typeparam>
    /// <typeparam name="TSortableProperties">A type that defines the filters allowed.</typeparam>
    /// <param name="validator"></param>
    /// <returns></returns>
    public static ListRequestValidationResult Validate<TSelectableProperties, TFilterableProperties, TSortableProperties>(this IListRequestValidator validator, IListRequest request, int? maxRecursionDepth = null)
    {
        var selectProperties = validator.GetProperties(typeof(TSelectableProperties), maxRecursionDepth: maxRecursionDepth);
        var filterProperties = validator.GetProperties(typeof(TFilterableProperties), maxRecursionDepth: maxRecursionDepth);
        var orderProperties = validator.GetProperties(typeof(TSortableProperties), maxRecursionDepth: maxRecursionDepth);
        return validator.Validate(request, selectProperties, filterProperties, orderProperties);
    }
}