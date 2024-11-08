namespace Csg.ListQuery.AspNetCore;

public class ListRequestOptions
{
    public int? MaxLimit { get; set; } = 100;

    public int? DefaultLimit { get; set; } = 25;

    /// <summary>
    /// Gets or sets the maximum recursion depth for which a list of valid field names will be generated from models. The default value is 1 (no recursion).
    /// </summary>
    public int MaximumRecursionDepth { get; set; } = 1;
}