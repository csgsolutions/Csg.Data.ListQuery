namespace Csg.ListQuery.Tests.Mock;

[Sortable]
public class Person
{
    [Filterable]
    public int PersonID { get; set; }

    [Filterable]
    [DbType(System.Data.DbType.AnsiString)]
    public string FirstName { get; set; }

    [Filterable]
    [DbType(System.Data.DbType.AnsiString)]
    public string LastName { get; set; }
}