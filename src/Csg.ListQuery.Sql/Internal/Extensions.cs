using Csg.Data.Sql;
using Csg.ListQuery.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Csg.ListQuery.Sql.Internal;

public static class Extensions
{
    /// <summary>
    /// Converts the filter operator into the <see cref="Csg.Data.Sql.SqlOperator"/> equivalent
    /// </summary>
    /// <param name="operator"></param>
    /// <returns></returns>
    public static Csg.Data.Sql.SqlOperator ToSqlOperator(this ListFilterOperator @operator)
    {
        Csg.Data.Sql.SqlOperator nativeOperator;

        switch (@operator)
        {
            case ListFilterOperator.Equal: nativeOperator = Csg.Data.Sql.SqlOperator.Equal; break;
            case ListFilterOperator.GreaterThan: nativeOperator = Csg.Data.Sql.SqlOperator.GreaterThan; break;
            case ListFilterOperator.GreaterThanOrEqual: nativeOperator = Csg.Data.Sql.SqlOperator.GreaterThanOrEqual; break;
            case ListFilterOperator.LessThan: nativeOperator = Csg.Data.Sql.SqlOperator.LessThan; break;
            case ListFilterOperator.LessThanOrEqual: nativeOperator = Csg.Data.Sql.SqlOperator.LessThanOrEqual; break;
            case ListFilterOperator.NotEqual: nativeOperator = Csg.Data.Sql.SqlOperator.NotEqual; break;
            default: throw new NotSupportedException($"An unsupported filter operator '{@operator}' was encountered");
        }

        return nativeOperator;
    }

    /// <summary>
    /// Creates a filter using the given generic filter operator
    /// </summary>
    /// <param name="where"></param>
    /// <param name="columnName"></param>
    /// <param name="operator"></param>
    /// <param name="value"></param>
    /// <param name="valueType"></param>
    /// <param name="valueTypeSize"></param>
    /// <param name="stringMatchType"></param>
    /// <param name="performDataTypeConversion"></param>
    /// <param name="valueConverter"></param>
    public static void AddFilter(this Csg.Data.IDbQueryWhereClause where, string columnName, ListFilterOperator @operator, object value, System.Data.DbType valueType, int? valueTypeSize = null, SqlWildcardDecoration stringMatchType = SqlWildcardDecoration.BeginsWith, bool performDataTypeConversion = true, Func<object, object> valueConverter = null)
    {
        var root = where.Root;
        IEnumerable<object> values = ValueHelpers.GetFilterValues(value, valueType, shouldPerformDataTypeConversion: performDataTypeConversion, valueConverter: valueConverter);
        bool isMutliValued = values != null && values.Count() > 1;
        var firstValue = values.Count() > 0 ? values.First() : null;
        var hasValues = values != null && values.Count() > 0;
        var dataType = valueType;
        int? dataTypeSize = valueTypeSize;

        if (!hasValues)
        {
            return;
        }

        //handle LIKE or EQUALS filtering against nchar and char data types
        if (dataType == DbType.AnsiStringFixedLength)
        {
            dataType = DbType.AnsiString;
            dataTypeSize = null;
        }
        else if (dataType == DbType.StringFixedLength)
        {
            dataType = DbType.String;
            dataTypeSize = null;
        }

        if (@operator == ListFilterOperator.Like)
        {
            if (isMutliValued)
            {
                var filterCollection = new Csg.Data.Sql.SqlFilterCollection();
                filterCollection.Logic = Csg.Data.Sql.SqlLogic.Or;
                foreach (var filterValue in values)
                {
                    filterCollection.Add(new Csg.Data.Sql.SqlStringMatchFilter(root, columnName, stringMatchType, filterValue.ToString())
                    {
                        DataType = dataType,
                        Size = dataTypeSize
                    });
                }
                where.AddFilter(filterCollection);
            }
            else if (firstValue == null)
            {
                where.AddFilter(new Csg.Data.Sql.SqlNullFilter(root, columnName, true));
            }
            else
            {
                where.AddFilter(new Csg.Data.Sql.SqlStringMatchFilter(root, columnName, stringMatchType, firstValue.ToString())
                {
                    DataType = dataType,
                    Size = dataTypeSize
                });
            }
        }
        else if (@operator == ListFilterOperator.Between)
        {
            if (!isMutliValued || values.Count() != 2)
            {
                throw new Exception(string.Format("Error while processing the filter values for column '{0}': A between filter must have exactly two values.", columnName));
            }

            var valueArray = values.ToArray();

            if (valueArray[0] != null)
            {
                where.AddFilter(new Csg.Data.Sql.SqlCompareFilter(root, columnName, Csg.Data.Sql.SqlOperator.GreaterThanOrEqual, dataType, valueArray[0]) { Size = dataTypeSize });
            }

            if (valueArray[1] != null)
            {
                where.AddFilter(new Csg.Data.Sql.SqlCompareFilter(root, columnName, Csg.Data.Sql.SqlOperator.LessThanOrEqual, dataType, valueArray[1]) { Size = dataTypeSize });
            }
        }
        else if (@operator == ListFilterOperator.IsNull)
        {
            var filterCollection = new Csg.Data.Sql.SqlFilterCollection();

            filterCollection.Logic = Csg.Data.Sql.SqlLogic.Or;
            foreach (var filterValue in values)
            {
                filterCollection.Add(new Csg.Data.Sql.SqlNullFilter(root, columnName, Convert.ToBoolean(filterValue))
                {
                    DataType = dataType
                });
            }

            where.AddFilter(filterCollection);
        }
        else
        {
            var nativeOperator = @operator.ToSqlOperator();

            // Handle filtering on a list for exact matches 
            if (isMutliValued && nativeOperator == Csg.Data.Sql.SqlOperator.Equal)
            {
                where.AddFilter(new Csg.Data.Sql.SqlListFilter(root, columnName, dataType, values) { Size = dataTypeSize });
            }
            // or not-in
            else if (isMutliValued && nativeOperator == Csg.Data.Sql.SqlOperator.NotEqual)
            {
                where.AddFilter(new Csg.Data.Sql.SqlListFilter(root, columnName, dataType, values) { Size = dataTypeSize, NotInList = true });
            }
            // handle filtering on a list for other operators (rare case)
            else if (isMutliValued)
            {
                var filterCollection = new Csg.Data.Sql.SqlFilterCollection();

                filterCollection.Logic = Csg.Data.Sql.SqlLogic.Or;

                foreach (var filterValue in values)
                {
                    filterCollection.Add(root, columnName, nativeOperator, dataType, filterValue);
                }

                where.AddFilter(filterCollection);
            }
            // handle filtering on NULL                        
            else if (firstValue == null)
            {
                //TODO: Should we handle filtering on null this way, or add an explicit IsNull on the filter object?
                where.AddFilter(new Csg.Data.Sql.SqlNullFilter(root, columnName, @operator == ListFilterOperator.Equal ? true : false));
            }
            // handle filtering on exactly one value
            else
            {
                where.AddFilter(new Csg.Data.Sql.SqlCompareFilter(root, columnName, nativeOperator, dataType, firstValue) { Size = dataTypeSize });
            }
        }
    }


}