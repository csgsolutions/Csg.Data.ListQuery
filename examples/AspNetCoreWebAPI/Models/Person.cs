using Csg.ListQuery;

namespace AspNetCoreWebAPI;

public class Person
{
    [Filterable]
    public int PersonID { get; set; }

    [Sortable]
    public string FirstName { get; set; }

    [Sortable]
    public string LastName { get; set; }
}