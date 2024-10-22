using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csg.ListQuery.Client;

namespace Csg.ListQuery.AspNetCore.Tests;

[TestClass]
public partial class ListRequestBuilderExtensionsTests
{
    [TestMethod]
    public void Test_BuildRequest()
    {
        var builder = new Mock.MockRequestBuilder<Person>();
        var request = builder.Select("FirstName", "LastName")
            .Where("FirstName", "Bob")
            .Where("Age", ListFilterOperator.GreaterThan, 1)
            .Order("LastName", "-FirstName")
            .Offset(25)
            .Limit(50)
            .ToQueryString();

        string expected = "?fields=FirstName,LastName&order=LastName,-FirstName&offset=25&limit=50&where[FirstName]=eq:Bob&where[Age]=gt:1";

        Assert.AreEqual(expected, request);
    }
}