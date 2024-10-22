using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Csg.Data.Sql;
using System.Linq;
using Csg.Data;
using Csg.ListQuery.Sql;
using System.Data;
using Dapper;

namespace Csg.ListQuery.Tests;

[TestClass]
public partial class ListQueryTests
{
    static ListQueryTests()
    {
        Csg.Data.DbQueryBuilder.GenerateFormattedSql = false;
    }

    [TestMethod]
    public void Test_ListQuery_BuildThrowsExceptionWhenNoConfig()
    {
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("Person", new Mock.MockConnection());
        var request = new ListQueryDefinition();

        request.Filters = new List<ListFilter>(new ListFilter[] {
            new ListFilter() { Name = "FirstName", Operator = ListFilterOperator.Equal, Value = "Bob" }
        });

        Assert.ThrowsException<System.Exception>(() =>
        {
            var stmt = ListQueryBuilder.Create(query, request)
                .Apply();
        });
    }

    [TestMethod]
    public void Test_ListQuery_BuildWithNoValidation()
    {
        var expectedSql = "SELECT [t0].[FirstName],[t0].[LastName] FROM [dbo].[Person] AS [t0] WHERE ([t0].[FirstName]=@p0);";
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("dbo.Person", new Mock.MockConnection());
        var queryDef = new ListQueryDefinition();

        queryDef.Fields = new string[] { "FirstName", "LastName" };
        queryDef.Filters = new List<ListFilter>(new ListFilter[] {
            new ListFilter() { Name = "FirstName", Operator = ListFilterOperator.Equal, Value = "Bob" }
        });

        var stmt = ListQueryBuilder.Create(query, queryDef)
            .NoValidation()
            .UseStreamingResult()
            .Apply()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText);
        Assert.AreEqual(1, stmt.Parameters.Count);
    }

    [TestMethod]
    public void Test_ListQuery_DefaultHandler()
    {
        var expectedSql = "SELECT [t0].[FirstName],[t0].[LastName] FROM [dbo].[Person] AS [t0] WHERE ([t0].[FirstName]=@p0);";
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("dbo.Person", new Mock.MockConnection());
        var queryDef = new ListQueryDefinition();

        queryDef.Fields = new string[] { "FirstName", "LastName" };
        queryDef.Filters = new List<ListFilter>(new ListFilter[] {
            new ListFilter() { Name = "FirstName", Operator = ListFilterOperator.Equal, Value = "Bob" }
        });

        var conn = new Mock.MockConnection();

        var stmt = conn
            .QueryBuilder("dbo.Person")
            .ListQuery(queryDef)
            .ValidateWith<Mock.Person>()
            .Apply()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText);
        Assert.AreEqual(1, stmt.Parameters.Count);
    }

    [TestMethod]
    public void Test_ListFilterOperator_Equals()
    {
        var expectedSql = "SELECT * FROM [dbo].[Person] AS [t0] WHERE ([t0].[FirstName]=@p0);";
        var queryDef = new ListQueryDefinition()
        {
            Filters = new List<ListFilter>() {
                new ListFilter() { Name = "FirstName", Operator = ListFilterOperator.Equal, Value = "Bob" }
            }
        };

        var stmt = new Mock.MockConnection().QueryBuilder("dbo.Person")
            .ListQuery(queryDef)
            .ValidateWith<Mock.Person>()
            .Apply()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText);
        Assert.AreEqual(1, stmt.Parameters.Count);
    }

    [TestMethod]
    public void Test_ListFilterOperator_NotEquals()
    {
        var expectedSql = "SELECT * FROM [dbo].[Person] AS [t0] WHERE ([t0].[FirstName]<>@p0);";
        var queryDef = new ListQueryDefinition()
        {
            Filters = new List<ListFilter>() {
                new ListFilter() { Name = "FirstName", Operator = ListFilterOperator.NotEqual, Value = "Bob" }
            }
        };

        var stmt = new Mock.MockConnection().QueryBuilder("dbo.Person")
            .ListQuery(queryDef)
            .ValidateWith<Mock.Person>()
            .Apply()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText);
        Assert.AreEqual(1, stmt.Parameters.Count);
    }

    [TestMethod]
    public void Test_ListFilterOperator_GreaterThan()
    {
        var expectedSql = "SELECT * FROM [dbo].[Person] AS [t0] WHERE ([t0].[FirstName]>@p0);";
        var queryDef = new ListQueryDefinition()
        {
            Filters = new List<ListFilter>() {
                new ListFilter() { Name = "FirstName", Operator = ListFilterOperator.GreaterThan, Value = "Bob" }
            }
        };

        var stmt = new Mock.MockConnection().QueryBuilder("dbo.Person")
            .ListQuery(queryDef)
            .ValidateWith<Mock.Person>()
            .Apply()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText);
        Assert.AreEqual(1, stmt.Parameters.Count);
    }

    [TestMethod]
    public void Test_ListFilterOperator_GreaterThanOrEqual()
    {
        var expectedSql = "SELECT * FROM [dbo].[Person] AS [t0] WHERE ([t0].[FirstName]>=@p0);";
        var queryDef = new ListQueryDefinition()
        {
            Filters = new List<ListFilter>() {
                new ListFilter() { Name = "FirstName", Operator = ListFilterOperator.GreaterThanOrEqual, Value = "Bob" }
            }
        };

        var stmt = new Mock.MockConnection().QueryBuilder("dbo.Person")
            .ListQuery(queryDef)
            .ValidateWith<Mock.Person>()
            .Apply()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText);
        Assert.AreEqual(1, stmt.Parameters.Count);
    }

    [TestMethod]
    public void Test_ListFilterOperator_LessThan()
    {
        var expectedSql = "SELECT * FROM [dbo].[Person] AS [t0] WHERE ([t0].[FirstName]<@p0);";
        var queryDef = new ListQueryDefinition()
        {
            Filters = new List<ListFilter>() {
                new ListFilter() { Name = "FirstName", Operator = ListFilterOperator.LessThan, Value = "Bob" }
            }
        };

        var stmt = new Mock.MockConnection().QueryBuilder("dbo.Person")
            .ListQuery(queryDef)
            .ValidateWith<Mock.Person>()
            .Apply()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText);
        Assert.AreEqual(1, stmt.Parameters.Count);
    }

    [TestMethod]
    public void Test_ListFilterOperator_LessThanOrEqual()
    {
        var expectedSql = "SELECT * FROM [dbo].[Person] AS [t0] WHERE ([t0].[FirstName]<=@p0);";
        var queryDef = new ListQueryDefinition()
        {
            Filters = new List<ListFilter>() {
                new ListFilter() { Name = "FirstName", Operator = ListFilterOperator.LessThanOrEqual, Value = "Bob" }
            }
        };

        var stmt = new Mock.MockConnection().QueryBuilder("dbo.Person")
            .ListQuery(queryDef)
            .ValidateWith<Mock.Person>()
            .Apply()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText);
        Assert.AreEqual(1, stmt.Parameters.Count);
    }

    [TestMethod]
    public void Test_ListFilterOperator_IsNullTrue()
    {
        var expectedSql = "SELECT * FROM [dbo].[Person] AS [t0] WHERE ([t0].[FirstName] IS NULL);";
        var queryDef = new ListQueryDefinition()
        {
            Filters = new List<ListFilter>() {
                new ListFilter() { Name = "FirstName", Operator = ListFilterOperator.IsNull, Value = true }
            }
        };

        var stmt = new Mock.MockConnection().QueryBuilder("dbo.Person")
            .ListQuery(queryDef)
            .ValidateWith<Mock.Person>()
            .Apply()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText);
    }

    [TestMethod]
    public void Test_ListFilterOperator_IsNullFalse()
    {
        var expectedSql = "SELECT * FROM [dbo].[Person] AS [t0] WHERE ([t0].[FirstName] IS NOT NULL);";
        var queryDef = new ListQueryDefinition()
        {
            Filters = new List<ListFilter>() {
                new ListFilter() { Name = "FirstName", Operator = ListFilterOperator.IsNull, Value = false }
            }
        };

        var stmt = new Mock.MockConnection().QueryBuilder("dbo.Person")
            .ListQuery(queryDef)
            .ValidateWith<Mock.Person>()
            .Apply()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText);
    }

    [TestMethod]
    public void Test_ListFilterOperator_Between()
    {
        var expectedSql = "SELECT * FROM [dbo].[Person] AS [t0] WHERE (([t0].[FirstName]>=@p0) AND ([t0].[FirstName]<=@p1));";
        var queryDef = new ListQueryDefinition()
        {
            Filters = new List<ListFilter>() {
                new ListFilter() { Name = "FirstName", Operator = ListFilterOperator.Between, Value = new string[]{ "Bob", "Dole" } }
            }
        };

        var stmt = new Mock.MockConnection().QueryBuilder("dbo.Person")
            .ListQuery(queryDef)
            .ValidateWith<Mock.Person>()
            .Apply()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText);
        Assert.AreEqual(2, stmt.Parameters.Count);
    }

    [TestMethod]
    public void Test_ListFilterOperator_Like()
    {
        var expectedSql = "SELECT * FROM [dbo].[Person] AS [t0] WHERE ([t0].[FirstName] LIKE @p0);";
        var queryDef = new ListQueryDefinition()
        {
            Filters = new List<ListFilter>() {
                new ListFilter() { Name = "FirstName", Operator = ListFilterOperator.Like, Value = "Bob" }
            }
        };

        var stmt = new Mock.MockConnection().QueryBuilder("dbo.Person")
            .ListQuery(queryDef)
            .ValidateWith<Mock.Person>()
            .Apply()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText);
        Assert.AreEqual(1, stmt.Parameters.Count);
        // default is a beginswith search
        Assert.AreEqual("Bob%", stmt.Parameters.First().Value.ToString());
    }

    [TestMethod]
    public void Test_ListQuery_InlineHandler()
    {
        var expectedSql = "SELECT * FROM [dbo].[Person] AS [t0] WHERE ([t0].[PersonID] IN (SELECT [t1].[PersonID] FROM [dbo].[PersonPhoneNumber] AS [t1] WHERE ([t1].[PhoneNumber] LIKE @p0) AND ([t1].[PersonID]=[t0].[PersonID])));";
        //                 SELECT * FROM [dbo].[Person] AS [t0] WHERE ([t0].[PersonID] IN (SELECT [t1].[PersonID] FROM [dbo].[PersonPhoneNumber] AS [t1] WHERE ([t1].[PhoneNumber] LIKE @p0) AND ([t1].[PersonID]=[t0].[PersonID])));
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("dbo.Person", new Mock.MockConnection());
        var queryDef = new ListQueryDefinition();

        queryDef.Filters = new List<ListFilter>(new ListFilter[] {
            new ListFilter() { Name = "PhoneNumber", Operator = ListFilterOperator.Like, Value = "555" }
        });

        var stmt = ListQueryBuilder.Create(query, queryDef)
            .ValidateWith<Mock.Person>()
            .AddFilterHandler("PhoneNumber", Mock.PersonFilters.PhoneNumber)
            .Apply()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText);
        Assert.AreEqual(1, stmt.Parameters.Count);
    }

    [TestMethod]
    public void Test_ListQuery_TypeDerivedHandler()
    {
        var expectedSql = "SELECT * FROM [dbo].[Person] AS [t0] WHERE ([t0].[PersonID] IN (SELECT [t1].[PersonID] FROM [dbo].[PersonPhoneNumber] AS [t1] WHERE ([t1].[PhoneNumber] LIKE @p0) AND ([t1].[PersonID]=[t0].[PersonID])));";
        //                 SELECT * FROM [dbo].[Person] AS [t0] WHERE ([t0].[PersonID] IN (SELECT [t1].[PersonID] FROM [dbo].[PersonPhoneNumber] AS [t1] WHERE ([t1].[PhoneNumber] LIKE @p0) AND ([t1].[PersonID]=[t0].[PersonID])));
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("dbo.Person", new Mock.MockConnection());
        var queryDef = new ListQueryDefinition();

        queryDef.Filters = new List<ListFilter>(new ListFilter[] {
            new ListFilter() { Name = "PhoneNumber", Operator = ListFilterOperator.Like, Value = "555" }
        });

        var stmt = ListQueryBuilder.Create(query, queryDef)
            .ValidateWith<Mock.Person>()
            .AddFilterHandlers<Mock.PersonFilters>()
            .Apply()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText);
        Assert.AreEqual(1, stmt.Parameters.Count);
    }

    [TestMethod]
    public void Test_ListQuery_Paging()
    {
        var expectedSql = "SELECT COUNT(1) FROM (SELECT * FROM [dbo].[Person] AS [t0] WHERE ([t0].[FirstName]=@p0)) AS [t0];\r\nSELECT * FROM [dbo].[Person] AS [t1] WHERE ([t1].[FirstName]=@p0) ORDER BY [FirstName] ASC OFFSET 0 ROWS FETCH NEXT 26 ROWS ONLY;";
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("dbo.Person", new Mock.MockConnection());

        var queryDef = new ListQueryDefinition();

        queryDef.Order = new List<SortField>()
        {
            new SortField(){ Name = "FirstName" }
        };

        queryDef.Filters = new List<ListFilter>()
        {
            new ListFilter(){ Name = "FirstName", Operator = ListFilterOperator.Equal, Value = "Bob"}
        };
             

        queryDef.Limit = 25;
        queryDef.Offset = 0; 

        var stmt = ListQueryBuilder.Create(query, queryDef)
            .NoValidation()
            .Render();

        Assert.AreEqual(expectedSql, stmt.CommandText.Trim(), true);
    }

    [TestMethod]
    public void Test_ListQuery_Buffered_ApplyAddsLimitOracle()
    {
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("dbo.Person", new Mock.MockConnection());

        var queryDef = new ListQueryDefinition();

        queryDef.Order = new List<SortField>()
        {
            new SortField(){ Name = "FirstName" }
        };

        queryDef.Offset = 0;
        queryDef.Limit = 50;

        var qb = ListQueryBuilder.Create(query, queryDef)
            .NoValidation()
            .Apply();

        Assert.AreEqual(0, qb.PagingOptions.Value.Offset);
        Assert.AreEqual(51, qb.PagingOptions.Value.Limit);
    }

    [TestMethod]
    public void Test_ListQuery_Streamed_Apply()
    {
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("dbo.Person", new Mock.MockConnection());

        var queryDef = new ListQueryDefinition();

        queryDef.Order = new List<SortField>()
        {
            new SortField(){ Name = "FirstName" }
        };

        queryDef.Offset = 0;
        queryDef.Limit = 50;

        var qb = ListQueryBuilder.Create(query, queryDef)
            .NoValidation()
            .UseStreamingResult()
            .Apply();

        Assert.AreEqual(0, qb.PagingOptions.Value.Offset);
        Assert.AreEqual(50, qb.PagingOptions.Value.Limit);
    }
         
    [TestMethod]
    public void Test_ListQuery_ApplyEventHandlers()
    {
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("dbo.Person", new Mock.MockConnection());
        var queryDef = new ListQueryDefinition();
        bool beforeInvoked = false;
        bool afterInvoked = false;

        queryDef.Order = new List<SortField>()
        {
            new SortField(){ Name = "FirstName" }
        };

        var qb = ListQueryBuilder.Create(query, queryDef)
            .NoValidation()
            .BeforeApply((config) =>
            {
                beforeInvoked = true;
                Assert.IsNotNull(config);
                Assert.AreEqual(0, config.QueryBuilder.OrderBy.Count);
                config.QueryBuilder.OrderBy.Add("LastName");
            })
            .AfterApply((config, appliedQuery) =>
            {
                afterInvoked = true;
                Assert.IsNotNull(config);
                Assert.IsNotNull(appliedQuery);
                Assert.AreEqual(2, appliedQuery.OrderBy.Count);
                Assert.IsTrue(appliedQuery.OrderBy.Any(x => x.ColumnName == "LastName"));
            })
            .Apply();

        Assert.IsTrue(beforeInvoked);
        Assert.IsTrue(afterInvoked);
        Assert.AreEqual(2, qb.OrderBy.Count);
    }

    [TestMethod]
    public void Test_ListQuery_DefaultSort()
    {
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("dbo.Person", new Mock.MockConnection());
        var queryDef = new ListQueryDefinition();

        var qb = ListQueryBuilder.Create(query, queryDef)
            .NoValidation()
            .DefaultSort("FirstName")
            .Apply();

        Assert.AreEqual(1, qb.OrderBy.Count);
        Assert.AreEqual("FirstName", qb.OrderBy.First().ColumnName);
    }

    [TestMethod]
    public void Test_ListQuery_DefaultLimit()
    {
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("dbo.Person", new Mock.MockConnection());
        var queryDef = new ListQueryDefinition();

        var qb = ListQueryBuilder.Create(query, queryDef)
            .NoValidation()
            .BeforeApply((config)=> config.UseLimitOracle=false)
            .DefaultLimit(150)
            .Apply();

        Assert.AreEqual(150, qb.PagingOptions.Value.Limit);
    }


    [TestMethod]
    public void ReflectionHelper_GetListPropertyInfo_DbTypeMappingsCorrect()
    {
        var properties = Csg.ListQuery.Internal.ReflectionHelper.GetFieldsFromType(typeof(Mock.TypeCheckModel))
            .ToDictionary(s => s.Name);

        Assert.AreEqual(DbType.Byte, properties[nameof(Mock.TypeCheckModel.Byte)].DataType);
        Assert.AreEqual(DbType.Int16, properties[nameof(Mock.TypeCheckModel.Int16)].DataType);
        Assert.AreEqual(DbType.Int32, properties[nameof(Mock.TypeCheckModel.Int32)].DataType);
        Assert.AreEqual(DbType.Int32, properties[nameof(Mock.TypeCheckModel.NullableInt32)].DataType);
        Assert.AreEqual(DbType.Int64, properties[nameof(Mock.TypeCheckModel.Int64)].DataType);
        Assert.AreEqual(DbType.Boolean, properties[nameof(Mock.TypeCheckModel.Bool)].DataType);
        Assert.AreEqual(DbType.Guid, properties[nameof(Mock.TypeCheckModel.Guid)].DataType);
        Assert.AreEqual(DbType.DateTime2, properties[nameof(Mock.TypeCheckModel.DateTime)].DataType);
        Assert.AreEqual(DbType.DateTimeOffset, properties[nameof(Mock.TypeCheckModel.DateTimeOffset)].DataType);
        Assert.AreEqual(DbType.Time, properties[nameof(Mock.TypeCheckModel.TimeSpan)].DataType);
        Assert.AreEqual(DbType.String, properties[nameof(Mock.TypeCheckModel.String)].DataType);
        Assert.AreEqual(DbType.StringFixedLength, properties[nameof(Mock.TypeCheckModel.Char)].DataType);
        Assert.AreEqual(DbType.Binary, properties[nameof(Mock.TypeCheckModel.ByteArray)].DataType);
        Assert.AreEqual(DbType.Decimal, properties[nameof(Mock.TypeCheckModel.Decimal)].DataType);
        Assert.AreEqual(DbType.Single, properties[nameof(Mock.TypeCheckModel.Float)].DataType);
        Assert.AreEqual(DbType.Double, properties[nameof(Mock.TypeCheckModel.Double)].DataType);
    }

    [TestMethod]
    public void ReflectionHelper_GetListPropertyInfo_Recursion()
    {
        var properties = Csg.ListQuery.Internal.ReflectionHelper.GetFieldsFromType(typeof(Mock.TypeCheckModel), fromCache: false, maxRecursionDepth: 2)
            .ToDictionary(s => s.Name);

        Assert.IsTrue(properties["Person.PersonID"].IsFilterable.GetValueOrDefault());
        Assert.IsTrue(properties["Person.FirstName"].IsFilterable.GetValueOrDefault());
        Assert.IsTrue(properties["Person.LastName"].IsFilterable.GetValueOrDefault());
        Assert.IsTrue(properties["Person.FirstName"].IsSortable.GetValueOrDefault());
        Assert.IsTrue(properties["Person.LastName"].IsSortable.GetValueOrDefault());
    }

    [TestMethod]
    public void ReflectionHelper_GetListPropertyInfo_NoRecursion()
    {
        var properties = Csg.ListQuery.Internal.ReflectionHelper.GetFieldsFromType(typeof(Mock.TypeCheckModel), fromCache: false, maxRecursionDepth: 1)
            .ToDictionary(s => s.Name);

        Assert.IsFalse(properties.ContainsKey("Person.PersonID"));
    }

    [TestMethod]
    public void ReflectionHelper_GetListPropertyInfo_TestDataContract()
    {
        var properties = Csg.ListQuery.Internal.ReflectionHelper.GetFieldsFromType(typeof(Mock.DataContractTestModel), fromCache: false, maxRecursionDepth: 1)
            .ToDictionary(s => s.Name);

        Assert.IsTrue(properties.ContainsKey("DataMember1"));
        Assert.IsFalse(properties.ContainsKey("NonDataMember1"));
        Assert.IsTrue(properties.ContainsKey("NonDataMember2"));
        Assert.IsTrue(properties.ContainsKey("NonDataMember3"));
    }

    [TestMethod]
    public void Test_ListQuery_FluentWithParametersFromQueryBuilder()
    {
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("dbo.Person", new Mock.MockConnection())
            .AddParameter("@Foo", "Bar", DbType.String)
            .AddParameter("@Bar", "Baz", DbType.String);

        var queryDef = new ListQueryDefinition();

        queryDef.Limit = 10;
        queryDef.Offset = 0; 

        var dapperCmd = query.ListQuery(queryDef)
            .NoValidation()
            .DefaultSort("FirstName")
            .Render()
            .ToDapperCommand();

        Assert.AreEqual(2, (dapperCmd.Parameters as DynamicParameters).ParameterNames.Count());
    }

    [TestMethod]
    public void Test_ListQuery_FluentWithParametersFromQueryBuilderWithPrefix()
    {
        var stringvalue = @"Select 1 into Prefix;SELECT COUNT(1) FROM (SELECT * FROM [dbo].[Person] AS [t0]) AS [t0];
SELECT * FROM [dbo].[Person] AS [t1] ORDER BY [FirstName] OFFSET 0 ROWS FETCH NEXT 11 ROWS ONLY;";
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("dbo.Person", new Mock.MockConnection())
            .AddParameter("@Foo", "Bar", DbType.String)
            .AddParameter("@Bar", "Baz", DbType.String);

        var queryDef = new ListQueryDefinition();

        queryDef.Limit = 10;
        queryDef.Offset = 0;

        query.Prefix = "Select 1 into Prefix";

        var dapperCmd = query.ListQuery(queryDef)
            .NoValidation()
            .DefaultSort("FirstName")
            .Render()
            .ToDapperCommand();

        Assert.AreEqual(2, (dapperCmd.Parameters as DynamicParameters).ParameterNames.Count());
        //  Assert.AreEqual(stringvalue, dapperCmd.CommandText.Trim());
    }
    [TestMethod]
    public void Test_ListQuery_FluentWithParametersFromQueryBuilderWithSuffix()
    {
        var stringvalue = @"SELECT COUNT(1) FROM (SELECT * FROM [dbo].[Person] AS [t0]) AS [t0];
SELECT * FROM [dbo].[Person] AS [t1] ORDER BY [FirstName] OFFSET 0 ROWS FETCH NEXT 11 ROWS ONLY;
Select * From Suffix;";
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("dbo.Person", new Mock.MockConnection())
            .AddParameter("@Foo", "Bar", DbType.String)
            .AddParameter("@Bar", "Baz", DbType.String);

        var queryDef = new ListQueryDefinition();

        queryDef.Limit = 10;
        queryDef.Offset = 0;

        query.Suffix = "Select * From Suffix";

        var dapperCmd = query.ListQuery(queryDef)
            .NoValidation()
            .DefaultSort("FirstName")
            .Render()
            .ToDapperCommand();

        Assert.AreEqual(2, (dapperCmd.Parameters as DynamicParameters).ParameterNames.Count());
        //  Assert.AreEqual(stringvalue, dapperCmd.CommandText.Trim());
    }

    [TestMethod]
    public void Test_ListQuery_FluentWithParametersFromQueryBuilderWithPrefixAndSuffix()
    {
        var stringvalue = @"Select 1 into Prefix;SELECT COUNT(1) FROM (SELECT * FROM [dbo].[Person] AS [t0]) AS [t0];
SELECT * FROM [dbo].[Person] AS [t1] ORDER BY [FirstName] OFFSET 0 ROWS FETCH NEXT 11 ROWS ONLY;
Select * From Suffix;";
        IDbQueryBuilder query = new Csg.Data.DbQueryBuilder("dbo.Person", new Mock.MockConnection())
            .AddParameter("@Foo", "Bar", DbType.String)
            .AddParameter("@Bar", "Baz", DbType.String);

        var queryDef = new ListQueryDefinition();

        queryDef.Limit = 10;
        queryDef.Offset = 0;

        query.Prefix = "Select 1 into Prefix";
        query.Suffix = "Select * From Suffix";

        var dapperCmd = query.ListQuery(queryDef)
            .NoValidation()
            .DefaultSort("FirstName")
            .Render()
            .ToDapperCommand();

        Assert.AreEqual(2, (dapperCmd.Parameters as DynamicParameters).ParameterNames.Count());
        // Assert.AreEqual(stringvalue, dapperCmd.CommandText.Trim());
    }
}