using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csg.ListQuery.Server.Internal;
using System.Runtime.Serialization;

namespace Csg.ListQuery.Server;

/// <summary>
/// Represents a query for a list of objects that optionally specifies a set of fields to return, filters, and sort order.
/// </summary>
[DataContract]
public class ListRequest : IListRequest
{
    /// <summary>
    /// Gets or sets a collection of field names to return.
    /// </summary>
    [DataMember]
    public virtual IList<string> Fields
    {
        get
        {
            _fields = _fields ?? new List<string>();
            return _fields;
        }
        set
        {
            _fields = value;
        }
    }
    private IList<string> _fields;

    /// <summary>
    /// Gets or sets a collection of filters to apply.
    /// </summary>
    [DataMember]
    public virtual ICollection<Csg.ListQuery.ListFilter> Filters
    {
        get
        {
            _filters = _filters ?? new List<ListFilter>();
            return _filters;
        }
        set
        {
            _filters = value;
        }
    }
    private ICollection<ListFilter> _filters;

    /// <summary>
    /// Gets or sets a list of sort actions to apply.
    /// </summary>
    [DataMember]
    public virtual IList<Csg.ListQuery.SortField> Order
    {
        get
        {
            _order = _order ?? new List<SortField>();
            return _order;
        }
        set
        {
            _order = value;
        }
    }
    private IList<SortField> _order;

    /// <summary>
    /// Gets or sets the zero-based index of the first record in the result set that will be returned.
    /// </summary>
    [DataMember]
    public virtual int? Offset { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of records that will be returned in the result set.
    /// </summary>
    [DataMember]
    public virtual int? Limit { get; set; }
        
    /// <summary>
    /// Gets a querystring representation of the request
    /// </summary>
    /// <returns></returns>        
    public virtual string ToQueryString()
    {
        var query = new StringBuilder("?");

        if (this.Fields != null)
        {
            query.Append(UrlElements.c_fields).Append("=").Append(string.Join(",", this.Fields.Select(System.Uri.EscapeDataString))).Append("&");
        }

        if (this.Order != null)
        {
            query.Append(UrlElements.c_order).Append("=").Append(string.Join(",", this.Order.Select(s => string.Concat(s.SortDescending ? "-" : "", System.Uri.EscapeDataString(s.Name))))).Append("&");
        }

        if (this.Offset > 0)
        {
            query.Append($"offset={this.Offset}").Append("&");
        }

        if (this.Limit > 0)
        {
            query.Append($"limit={this.Limit}").Append("&");
        }

        if (this.Filters != null)
        {
            foreach (var filter in this.Filters)
            {

                // special case needs to make two filters and Value is an enumerable with two values
                if (filter.Operator == ListFilterOperator.Between)
                {
                    var filterValues = Csg.ListQuery.Internal.ValueHelpers.GetFilterValues(filter.Value, System.Data.DbType.String, true)
                        .Select(s => s.ToString());

                    query.Append($"where[{System.Uri.EscapeDataString(filter.Name)}]=ge:")
                        .Append(System.Uri.EscapeDataString(filterValues.First()))
                        .Append("&")
                        .Append($"where[{System.Uri.EscapeDataString(filter.Name)}]=le:")
                        .Append(System.Uri.EscapeDataString(filterValues.Skip(1).First()))
                        .Append("&");
                }
                else
                {
                    query.Append($"where[{System.Uri.EscapeDataString(filter.Name)}]=")
                        .Append(UrlElements.OperatorToString(filter.Operator))
                        .Append(":")
                        .Append(System.Uri.EscapeDataString(filter.Value.ToString()))
                        .Append("&");
                }
            }
        }

        // remove the last ampersand
        query.Length--;

        return query.ToString();
    }

    #region IListRequest

    IList<string> IListRequest.Fields { get => this.Fields; set => this.Fields = value; }

    ICollection<ListFilter> IListRequest.Filters { get => this.Filters; set => this.Filters = value; }

        
    IList<SortField> IListRequest.Order { get => this.Order; set => this.Order = value; }

    #endregion
}