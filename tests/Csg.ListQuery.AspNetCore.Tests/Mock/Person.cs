using System;

namespace Csg.ListQuery.AspNetCore.Tests;

[Sortable]
[Filterable]
public class Person
{
    public int PersonID { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTimeOffset BirthDate { get; set; }
}

public class PersonFilters
{
    [Filterable]
    public int PersonID { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    [Filterable(Description = "Filters on birth date")]
    public DateTimeOffset BirthDate { get; set; }
}

public class PersonSorts
{
    public int PersonID { get; set; }
        
    public string FirstName { get; set; }

    [Sortable]
    public string LastName { get; set; }

    [Sortable]
    public DateTimeOffset BirthDate { get; set; }
}