using System;

namespace Csg.ListQuery.Server.Internal;

public static class UrlElements
{
    public const string c_where = "where";
    public const string c_fields = "fields";
    public const string c_order = "order";
    public const string c_start = "offset";
    public const string c_limit = "limit";
    public const string op_eq = "eq";
    public const string op_gt = "gt";
    public const string op_ge = "ge";
    public const string op_lt = "lt";
    public const string op_le = "le";
    public const string op_ne = "ne";
    public const string op_like = "like";
    public const string op_isnull = "isnull";
    public const string op_in = "in";
    public const string op_nin = "nin";
    public const char c_colon = ':';

    /// <summary>
    /// Gets the ListQuery standard sstring representation of the given operator.
    /// </summary>
    /// <param name="oper"></param>
    /// <returns></returns>
    public static string OperatorToString(ListFilterOperator? oper)
    {
        if (!oper.HasValue)
        {
            return string.Empty;
        }

        switch (oper.Value)
        {
            case ListFilterOperator.Equal: return UrlElements.op_eq;
            case ListFilterOperator.NotEqual: return UrlElements.op_ne;
            case ListFilterOperator.GreaterThan: return UrlElements.op_gt;
            case ListFilterOperator.GreaterThanOrEqual: return UrlElements.op_ge;
            case ListFilterOperator.LessThan: return UrlElements.op_lt;
            case ListFilterOperator.LessThanOrEqual: return UrlElements.op_le;
            case ListFilterOperator.Like: return UrlElements.op_like;
            case ListFilterOperator.IsNull: return UrlElements.op_isnull;
        }

        throw new NotSupportedException($"Unsupported filter operator: {oper}");
    }
}