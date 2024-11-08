using Csg.Data;

namespace Csg.ListQuery.Tests.Mock;

public class PersonFilters
{
    public static void PhoneNumber(IDbQueryWhereClause where, ListFilter filter, ListFieldMetadata config)
    {
        var sqf = new Csg.Data.Sql.SqlSubQueryFilter(where.Root, "dbo.PersonPhoneNumber");
        sqf.ColumnName = "PersonID";
        sqf.SubQueryColumn = "PersonID";
        sqf.SubQueryFilters.Add(new Csg.Data.Sql.SqlStringMatchFilter(sqf.SubQueryTable, "PhoneNumber", Csg.Data.Sql.SqlWildcardDecoration.Contains, filter.Value.ToString()));
        sqf.SubQueryFilters.Add(new Csg.Data.Sql.SqlColumnCompareFilter(sqf.SubQueryTable, "PersonID", Csg.Data.Sql.SqlOperator.Equal, sqf.Table));
        where.AddFilter(sqf);
    }
}