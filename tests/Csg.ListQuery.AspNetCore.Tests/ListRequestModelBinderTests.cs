using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;
using Csg.ListQuery.Server;

namespace Csg.ListQuery.AspNetCore.Tests;

[TestClass]
public class ListRequestModelBinderTests
{
    [TestMethod]
    public void CreateRequest_FromString()
    {
        var queryString = "fields=PersonID,FirstName,LastName,BirthDate&where[firstName]=bob&order=LastName&order=-FirstName";
        var request = new ModelBinding.ListRequestFactory().CreateRequest<ListRequest>(queryString);

        Assert.AreEqual(4, request.Fields.Count());
        Assert.AreEqual("PersonID", request.Fields.First(), true);
        Assert.AreEqual("FirstName", request.Fields.Skip(1).First(), true);
        Assert.AreEqual("LastName", request.Fields.Skip(2).First(), true);
        Assert.AreEqual("BirthDate", request.Fields.Last(), true);

        Assert.AreEqual(1, request.Filters.Count());
        Assert.AreEqual("FirstName", request.Filters.First().Name, true);
        Assert.AreEqual("bob", request.Filters.First().Value);
        Assert.AreEqual(Csg.ListQuery.ListFilterOperator.Equal, request.Filters.First().Operator.Value);

        Assert.AreEqual(2, request.Order.Count());
        Assert.AreEqual("LastName", request.Order.First().Name, true);
        Assert.AreEqual("FirstName", request.Order.Last().Name, true);
        Assert.AreEqual(true, request.Order.Last().SortDescending);
    }

    [TestMethod]
    public void CreateRequest_FromString_TrimsFields()
    {
        var queryString = "fields=PersonID, FirstName, LastName,BirthDate";
        var request = new ModelBinding.ListRequestFactory().CreateRequest<ListRequest>(queryString);

        Assert.AreEqual(4, request.Fields.Count());
        Assert.AreEqual("PersonID", request.Fields.First(), true);
        Assert.AreEqual("FirstName", request.Fields.Skip(1).First(), true);
        Assert.AreEqual("LastName", request.Fields.Skip(2).First(), true);
        Assert.AreEqual("BirthDate", request.Fields.Last(), true);
    }

    //[TestMethod]
    //public void CreateRequest_UnsupportedPaging_Throws()
    //{
    //    var queryString = "offset=50&limit=10";

    //    Assert.ThrowsException<NotSupportedException>(() =>
    //    {
    //        new ModelBinding.ListRequestFactory().CreateRequest<ListRequest>(queryString);
    //    });            
    //}

    [TestMethod]
    public void CreateRequest_UnsupportedParameter_Throws()
    {
        var queryString = "fields=FirstName&foo=bar";

        Assert.ThrowsException<FormatException>(() =>
        {
            new ModelBinding.ListRequestFactory().CreateRequest<ListRequest>(queryString);
        });
    }

    [TestMethod]
    public void CreateRequest_FromStringWithPaging()
    {
        var queryString = "fields=PersonID,FirstName,LastName,BirthDate&where[firstName]=bob&order=LastName&order=-FirstName&offset=50&limit=10";
        var request = new ModelBinding.ListRequestFactory().CreateRequest<ListRequest>(queryString);

        Assert.AreEqual(4, request.Fields.Count());
        Assert.AreEqual("PersonID", request.Fields.First(), true);
        Assert.AreEqual("FirstName", request.Fields.Skip(1).First(), true);
        Assert.AreEqual("LastName", request.Fields.Skip(2).First(), true);
        Assert.AreEqual("BirthDate", request.Fields.Last(), true);

        Assert.AreEqual(1, request.Filters.Count());
        Assert.AreEqual("FirstName", request.Filters.First().Name, true);
        Assert.AreEqual("bob", request.Filters.First().Value);
        Assert.AreEqual(Csg.ListQuery.ListFilterOperator.Equal, request.Filters.First().Operator.Value);

        Assert.AreEqual(2, request.Order.Count());
        Assert.AreEqual("LastName", request.Order.First().Name, true);
        Assert.AreEqual("FirstName", request.Order.Last().Name, true);
        Assert.AreEqual(true, request.Order.Last().SortDescending);

        Assert.AreEqual(10, request.Limit);
        Assert.AreEqual(50, request.Offset);
    }
}