using Csg.ListQuery.Server;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Csg.ListQuery.AspNetCore;

/// <summary>
/// Default validator implementation
/// </summary>
public class DefaultListQueryValidator : IListRequestValidator, System.IDisposable
{
    private ListRequestOptions _options;
    private IDisposable _onChangeSubscription;
    public int? DefaultLimit { get { return _options.DefaultLimit; } }
    public int? MaxLimit { get { return _options.MaxLimit; } }

    /// <summary>
    /// Initializes a new instance
    /// </summary>
    public DefaultListQueryValidator(IOptionsMonitor<ListRequestOptions> options)
    {
        _options = options.CurrentValue;
        _onChangeSubscription = options.OnChange((val) =>
        {
            _options = val;
        });
    }

    /// <summary>
    /// Creates a new query definition that will be populated with the filters, fields and sorts.
    /// </summary>
    /// <returns></returns>
    protected virtual Csg.ListQuery.ListQueryDefinition CreateQueryDefinition()
    {
        return new Csg.ListQuery.ListQueryDefinition();
    }

    public virtual IDictionary<string, ListItemPropertyInfo> GetProperties(Type type, Func<ListItemPropertyInfo, bool> predicate = null, int? maxRecursionDepth = null)
    {
        return PropertyHelper.GetProperties(type, predicate, maxRecursionDepth ?? _options.MaximumRecursionDepth);
    }

    /// <summary>
    /// Transforms a list request into a list query
    /// </summary>
    /// <param name="selectableProperties">A set of properties used to validate the given selections</param>
    /// <param name="filerableProperties">A set of properties used to validate the given filters</param>
    /// <param name="sortableProperties">A set of properties used to validate the given sorts</param>
    /// <exception cref="MissingFieldException">When a field is not valid</exception>
    /// <returns></returns>
    public virtual ListRequestValidationResult Validate(
        IListRequest request,
        IDictionary<string, ListItemPropertyInfo> selectableProperties,
        IDictionary<string, ListItemPropertyInfo> filerableProperties,
        IDictionary<string, ListItemPropertyInfo> sortableProperties
    )
    {
        var queryDef = CreateQueryDefinition();
        var errors = new List<ListRequestValidationError>();

        if (request.Fields != null)
        {
            queryDef.Fields = request.Fields.Select(s => new
                {
                    Raw = s,
                    Exists = selectableProperties.TryGetValue(s, out ListItemPropertyInfo domainProp),
                    Domain = domainProp
                }).Where(field =>
                {
                    if (field.Exists)
                    {
                        return true;
                    }
                    else
                    {
                        errors.Add(field.Raw, "Field is not valid for selection.");
                        return false;
                    }
                })
                .Select(field => field.Domain.PropertyName)
                .ToList();
        }

        if (request.Filters != null)
        {
            queryDef.Filters = request.Filters.Select(s => new
                {
                    Raw = s,
                    Exists = filerableProperties.TryGetValue(s.Name, out ListItemPropertyInfo domainProp),
                    Domain = domainProp
                }).Where(filter =>
                {
                    if (filter.Exists && filter.Domain.IsFilterable)
                    {
                        return true;
                    }
                    else
                    {
                        errors.Add(filter.Raw.Name, "Field is not valid for filtering.");
                        return false;
                    }
                }).Select(filter =>
                {
                    return new ListFilter()
                    {
                        Name = filter.Domain.PropertyName,
                        Operator = filter.Raw.Operator,
                        Value = filter.Raw.Value
                    };
                })
                .ToList();
        }

        if (request.Order != null)
        {
            queryDef.Order = request.Order.Select(s => new
                {
                    Raw = s,
                    Exists = sortableProperties.TryGetValue(s.Name, out ListItemPropertyInfo domainProp),
                    Domain = domainProp
                }).Where(s =>
                {
                    if (s.Exists && s.Domain.IsSortable)
                    {
                        return true;
                    }
                    else
                    {
                        errors.Add(s.Raw.Name, "Field is not valid for sorting.");
                        return false;
                    }
                }).Select(s =>
                {
                    return new SortField()
                    {
                        Name = s.Domain.PropertyName,
                        SortDescending = s.Raw.SortDescending
                    };
                })
                .ToList();
        }

        if (request.Offset < 0)
        {
            errors.Add("offset", $"Offset must be greater than or equal to 0.");
        }

        if (request.Limit > 0)
        {
            queryDef.Offset = request.Offset.GetValueOrDefault();
            queryDef.Limit = request.Limit.Value;
        }
        else if (request.Limit < 0)
        {
            errors.Add("limit", $"Limit must be greater than or equal to zero.");
        }

        if (_options.DefaultLimit.HasValue && queryDef.Limit <= 0)
        {
            queryDef.Limit = _options.DefaultLimit.Value;
        }

        if (_options.MaxLimit.HasValue && queryDef.Limit > _options.MaxLimit.Value)
        {
            errors.Add("limit", $"Limit must be greater than or equal to 0 and less than or equal to {_options.MaxLimit}");
        }

        return new ListRequestValidationResult(errors, queryDef);
    }

    public void Dispose()
    {
        _onChangeSubscription?.Dispose();
    }
}