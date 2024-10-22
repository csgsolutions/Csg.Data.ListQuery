using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Csg.ListQuery.Server;
using Csg.ListQuery.AspNetCore.Tests.Mock;

namespace Csg.ListQuery.AspNetCore.Tests;

[TestClass]
public class ListRequestTests
{
    [TestMethod]
    public void Test_PropertyHelper_GetDomainProperties()
    {
        var selectProps = PropertyHelper.GetProperties(typeof(Person));
        var filterProps = PropertyHelper.GetProperties(typeof(PersonFilters));
        var sortProps = PropertyHelper.GetProperties(typeof(PersonSorts));

        Assert.AreEqual(4, selectProps.Count);
        Assert.AreEqual(4, selectProps.Count(x => x.Value.IsSortable));
        Assert.AreEqual(4, selectProps.Count(x => x.Value.IsFilterable));

        Assert.AreEqual(4, filterProps.Count);
        Assert.AreEqual(2, filterProps.Count(x => x.Value.IsFilterable));

        Assert.AreEqual(4, sortProps.Count);
        Assert.AreEqual(2, sortProps.Count(x => x.Value.IsSortable));
    }

    [TestMethod]
    public void Test_DefaultValidator_Validate_AllArgs()
    {
        var options = new MockOptionsMonitor<ListRequestOptions>(new ListRequestOptions());
        var validator = new DefaultListQueryValidator(options);
        var selectProps = PropertyHelper.GetProperties(typeof(Person));
        var filterProps = PropertyHelper.GetProperties(typeof(PersonFilters), x => x.IsFilterable);
        var sortProps = PropertyHelper.GetProperties(typeof(PersonSorts), x => x.IsSortable);
           
        var request = new ListRequest();
        request.Fields = new string[] { "PersonID", "LastName", "FirstName", "BirthDate" };
        request.Filters = new List<ListFilter>
        {
            new ListQuery.ListFilter(){ Name  = "PersonID", Operator = ListFilterOperator.Equal, Value = "test" },
            new ListFilter(){ Name  = "LastName", Operator = ListFilterOperator.Equal, Value = "test" },
            new ListFilter(){ Name  = "FirstName", Operator = ListFilterOperator.Equal, Value = "test" },
            new ListFilter(){ Name  = "BirthDate", Operator = ListFilterOperator.Equal, Value = "test" },
        };
        request.Order = new List<SortField>
        {
            new SortField(){ Name  = "PersonID" },
            new SortField(){ Name  = "LastName" },
            new SortField(){ Name  = "FirstName" },
            new SortField(){ Name  = "BirthDate" },
        };

        var result = validator.Validate(request, selectProps, filterProps, sortProps);

        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(4, result.Errors.Count);

    }

    [TestMethod]
    public void Test_DefaultValidator_Validate_OneGenericArg()
    {
        var options = new MockOptionsMonitor<ListRequestOptions>(new ListRequestOptions());
        var validator = new DefaultListQueryValidator(options);
        var request = new ListRequest();
        request.Fields = new string[] { "PersonID","LastName","FirstName","BirthDate" };
        request.Filters = new List<ListFilter>
        {
            new ListFilter(){ Name  = "PersonID", Operator = ListFilterOperator.Equal, Value = "test" },
            new ListFilter(){ Name  = "LastName", Operator = ListFilterOperator.Equal, Value = "test" },
            new ListFilter(){ Name  = "FirstName", Operator = ListFilterOperator.Equal, Value = "test" },
            new ListFilter(){ Name  = "BirthDate", Operator = ListFilterOperator.Equal, Value = "test" },
        };
        request.Order = new List<SortField>
        {
            new SortField(){ Name  = "PersonID" },
            new SortField(){ Name  = "LastName" },
            new SortField(){ Name  = "FirstName" },
            new SortField(){ Name  = "BirthDate" },
        };

        var result = validator.Validate<Person>(request);

        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(4, result.ListQuery.Fields.Count());
        Assert.AreEqual(4, result.ListQuery.Filters.Count());
        Assert.AreEqual(4, result.ListQuery.Order.Count());
    }

    [TestMethod]
    public void Test_DefaultValidator_Validate_TwoGenericArgs()
    {
        var options = new MockOptionsMonitor<ListRequestOptions>(new ListRequestOptions());
        var validator = new DefaultListQueryValidator(options);
        var request = new ListRequest();
        request.Fields = new string[] { "PersonID", "LastName", "FirstName", "BirthDate" };
        request.Filters = new List<ListFilter>
        {
            new ListFilter(){ Name  = "PersonID", Operator = ListFilterOperator.Equal, Value = "test" },
            new ListFilter(){ Name  = "LastName", Operator = ListFilterOperator.Equal, Value = "test" },
            new ListFilter(){ Name  = "FirstName", Operator = ListFilterOperator.Equal, Value = "test" },
            new ListFilter(){ Name  = "BirthDate", Operator = ListFilterOperator.Equal, Value = "test" },
        };
        request.Order = new List<SortField>
        {
            new SortField(){ Name  = "PersonID" },
            new SortField(){ Name  = "LastName" },
            new SortField(){ Name  = "FirstName" },
            new SortField(){ Name  = "BirthDate" },
        };

        var result = validator.Validate<Person, PersonFilters>(request);

        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(2, result.Errors.Count);
        Assert.AreEqual(4, result.ListQuery.Fields.Count());
        Assert.AreEqual(2, result.ListQuery.Filters.Count());
        Assert.AreEqual(4, result.ListQuery.Order.Count());
    }

    [TestMethod]
    public void Test_DefaultValidator_Validate_ThreeGenericArgs()
    {
        var options = new MockOptionsMonitor<ListRequestOptions>(new ListRequestOptions());
        var validator = new DefaultListQueryValidator(options);
        var request = new ListRequest();
        request.Fields = new string[] { "PersonID", "LastName", "FirstName", "BirthDate" };
        request.Filters = new List<ListFilter>
        {
            new ListFilter(){ Name  = "PersonID", Operator = ListFilterOperator.Equal, Value = "test" },
            new ListFilter(){ Name  = "LastName", Operator = ListFilterOperator.Equal, Value = "test" },
            new ListFilter(){ Name  = "FirstName", Operator = ListFilterOperator.Equal, Value = "test" },
            new ListFilter(){ Name  = "BirthDate", Operator = ListFilterOperator.Equal, Value = "test" },
        };
        request.Order = new List<SortField>
        {
            new SortField(){ Name  = "PersonID" },
            new SortField(){ Name  = "LastName" },
            new SortField(){ Name  = "FirstName" },
            new SortField(){ Name  = "BirthDate" },
        };

        var result = validator.Validate<Person, PersonFilters, PersonSorts>(request);

        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(4, result.Errors.Count);
        Assert.AreEqual(4, result.ListQuery.Fields.Count());
        Assert.AreEqual(2, result.ListQuery.Filters.Count());
        Assert.AreEqual(2, result.ListQuery.Order.Count());
    }

    [TestMethod]
    public void Test_DefaultValidator_Validate_DefaultLimit()
    {
        var options = new MockOptionsMonitor<ListRequestOptions>(new ListRequestOptions() { DefaultLimit = 10 });
        var validator = new DefaultListQueryValidator(options);
        var request = new ListRequest();
        var result = validator.Validate<Person, PersonFilters, PersonSorts>(request);

        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(10, result.ListQuery.Limit);
    }

    [TestMethod]
    public void Test_DefaultValidator_Validate_MaxLimit()
    {
        var options = new MockOptionsMonitor<ListRequestOptions>(new ListRequestOptions() { MaxLimit = 10 });
        var validator = new DefaultListQueryValidator(options);
        var request = new ListRequest();
        request.Limit = 50;
            
        var result = validator.Validate<Person, PersonFilters, PersonSorts>(request);

        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
    }

    [TestMethod]
    public void Test_ListRequest_ToQueryString()
    {
        string expected = "?fields=PersonID,LastName,FirstName,BirthDate&order=PersonID,LastName,FirstName,BirthDate&offset=50&limit=25&where[PersonID]=eq:123&where[LastName]=lt:test&where[FirstName]=like:test&where[BirthDate]=gt:1900-01-01";
        //?fields=PersonID,LastName,FirstName,BirthDate&order=PersonID,LastName,FirstName,BirthDate&offset=50&limit=25&where[PersonID]=eq:123&where[LastName]=lt:test&where[FirstName]=like:test&where[BirthDate]=gt:1900-01-01
        var request = new ListRequest();
        request.Offset = 50;
        request.Limit = 25;
        request.Fields = new string[] { "PersonID", "LastName", "FirstName", "BirthDate" };
        request.Filters = new List<ListFilter>
        {
            new ListFilter(){ Name  = "PersonID", Operator = ListFilterOperator.Equal, Value = "123" },
            new ListFilter(){ Name  = "LastName", Operator = ListFilterOperator.LessThan, Value = "test" },
            new ListFilter(){ Name  = "FirstName", Operator = ListFilterOperator.Like, Value = "test" },
            new ListFilter(){ Name  = "BirthDate", Operator = ListFilterOperator.GreaterThan, Value = "1900-01-01" },
        };
        request.Order = new List<SortField>
        {
            new SortField(){ Name  = "PersonID" },
            new SortField(){ Name  = "LastName" },
            new SortField(){ Name  = "FirstName" },
            new SortField(){ Name  = "BirthDate" },
        };

        var result = request.ToQueryString();

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Test_DefaultValidator_Validate_RequestWithEmptyCollections_IsValid()
    {
        var options = new MockOptionsMonitor<ListRequestOptions>(new ListRequestOptions());
        var validator = new DefaultListQueryValidator(options);
        var selectProps = PropertyHelper.GetProperties(typeof(Person));
        var filterProps = PropertyHelper.GetProperties(typeof(PersonFilters), x => x.IsFilterable);
        var sortProps = PropertyHelper.GetProperties(typeof(PersonSorts), x => x.IsSortable);

        var request = new ListRequest();
        request.Fields = new string[] {};
        request.Filters = new List<ListFilter>();
        request.Order = new List<SortField>();

        var result = validator.Validate(request, selectProps, filterProps, sortProps);

        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }
}