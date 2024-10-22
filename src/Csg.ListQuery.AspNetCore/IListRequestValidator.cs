using Csg.ListQuery.Server;
using System;
using System.Collections.Generic;

namespace Csg.ListQuery.AspNetCore;

/// <summary>
/// Represents a class that validates a list request and produces a <see cref="ListQueryDefinition"/>.
/// </summary>
public interface IListRequestValidator
{
    int? DefaultLimit { get; }
    int? MaxLimit { get; }

    /// <summary>
    /// When implemented in a derived class, validates the given request and produces a <see cref="ListQueryDefinition"/> query.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="selectableProperties">A dictionary of the fields that are allowed to be selected.</param>
    /// <param name="filerableProperties">A dictionary of the fields that are allowed to be filtered.</param>
    /// <param name="sortableProperties">A dictionary of the fields that are allowed to be sorted.</param>
    /// <returns></returns>
    ListRequestValidationResult Validate(
        IListRequest request,
        IDictionary<string, ListItemPropertyInfo> selectableProperties,
        IDictionary<string, ListItemPropertyInfo> filerableProperties,
        IDictionary<string, ListItemPropertyInfo> sortableProperties
    );

    /// <summary>
    /// When implemented in a derived class, gets a list of properties on the given type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="predicate"></param>
    /// <param name="maxRecursionDepth"></param>
    IDictionary<string, ListItemPropertyInfo> GetProperties(Type type, Func<ListItemPropertyInfo, bool> predicate = null, int? maxRecursionDepth = null);
}