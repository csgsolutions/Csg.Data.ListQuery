namespace Csg.ListQuery;

/// <summary>
/// Represents the operator to be applied in a comparison.
/// </summary>
public enum ListFilterOperator
{
    Equal = 0,
    NotEqual = 1,
    GreaterThan = 2,
    GreaterThanOrEqual = 3,
    LessThan = 4,
    LessThanOrEqual = 5,
    Between = 6,
    Like = 7,
    IsNull = 8
}