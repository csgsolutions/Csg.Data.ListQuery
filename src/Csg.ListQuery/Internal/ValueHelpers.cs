using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Csg.ListQuery.Internal;

public static class ValueHelpers
{
    /// <summary>
    /// Gets a value that indicates if <see cref="ListFilter.Value"/> is a type derived from <see cref="System.Collections.IEnumerable"/>, and therefore contains multiple values.
    /// </summary>
    /// <returns></returns>
    internal static bool IsEnumerable(object value)
    {
        return (!(value is string)) && (value is System.Collections.IEnumerable);
    }

    /// <summary>
    /// Returns the given value converted to the given data type and optional value converter.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="dbType"></param>
    /// <param name="shouldPerformDataTypeConversion"></param>
    /// <param name="valueConverter"></param>
    /// <returns></returns>
    internal static object GetFilterValue(object value, DbType dbType, bool shouldPerformDataTypeConversion = true, Func<object, object> valueConverter = null)
    {
        return ConvertValue(value, dbType,
            shouldPerformDataTypeConversion: shouldPerformDataTypeConversion,
            valueConverter: valueConverter);
    }

    /// <summary>
    /// Returns the given value cast as an IEnumerable of the given type and executes the given value converter over each value.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<object> GetFilterValues(object value, DbType dbType, bool shouldPerformDataTypeConversion = true, Func<object, object> valueConverter = null)
    {
        if (IsEnumerable(value))
        {
            return ((System.Collections.IEnumerable)value).Cast<object>().Select(s => ConvertValue(s, dbType,
                shouldPerformDataTypeConversion: shouldPerformDataTypeConversion,
                valueConverter: valueConverter));
        }
        else
        {
            return new object[] { GetFilterValue(value, dbType, shouldPerformDataTypeConversion, valueConverter) };
        }
    }

    /// <summary>
    /// Convert the value to a given dbtype
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="dbType"></param>
    /// <param name="shouldPerformDataTypeConversion"></param>
    /// <param name="valueConverter"></param>
    /// <returns></returns>
    public static object ConvertValue<T>(T value, DbType dbType, bool shouldPerformDataTypeConversion = true, Func<object, object> valueConverter = null)
    {
        object outValue = value;

        if (value is DBNull)
        {
            outValue = null;
        }

        //if (outValue is Newtonsoft.Json.Linq.JToken && ((Newtonsoft.Json.Linq.JToken)outValue).Type == Newtonsoft.Json.Linq.JTokenType.Null)
        //{
        //    outValue = null;
        //}

        if (outValue != null && shouldPerformDataTypeConversion)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    outValue = Convert.ToString(value); break;
                case DbType.Byte:
                    outValue = Convert.ToByte(value); break;
                case DbType.Currency:
                    outValue = Convert.ToDecimal(value); break;
                case DbType.Date:
                    outValue = Convert.ToDateTime(value).Date; break;
                case DbType.DateTime:
                    outValue = Convert.ToDateTime(value); break;
                case DbType.DateTime2:
                    outValue = Convert.ToDateTime(value); break;
                case DbType.DateTimeOffset:
                    outValue = DateTimeOffset.Parse(value.ToString()); break;
                case DbType.Time:
                    outValue = TimeSpan.Parse(value.ToString()); break;
                case DbType.Decimal:
                    outValue = Convert.ToDecimal(value); break;
                case DbType.Double:
                    outValue = Convert.ToDouble(value); break;
                case DbType.Int16:
                    outValue = Convert.ToInt16(value); break;
                case DbType.Int32:
                    outValue = Convert.ToInt32(value); break;
                case DbType.Int64:
                    outValue = Convert.ToInt64(value); break;
                case DbType.Single:
                    outValue = Convert.ToSingle(value); break;
                case DbType.Boolean:
                    outValue = Convert.ToBoolean(value); break;
                case DbType.Guid:
                    outValue = (value is Guid) ? (object)value : new Guid(value.ToString()); break;
                default:
                    throw new NotSupportedException("Unsupported data type: " + dbType.ToString());
            }
        }

        if (valueConverter != null)
        {
            outValue = valueConverter(outValue);
        }

        return outValue;
    }
}