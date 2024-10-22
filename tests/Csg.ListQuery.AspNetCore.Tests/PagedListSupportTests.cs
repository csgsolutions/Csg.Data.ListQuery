using Csg.ListQuery.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Csg.ListQuery.AspNetCore.Tests;

[TestClass]
public class PagedListSupportTests
{
    [TestMethod]
    public async Task Test_GetAllPagesAsync()
    {
        var client = new MockClient();
        for (int i = 1; i <= 105; i++)
        {
            client.People.Add(new Person());
        }

        string uri = "https://localhost/api/People/filter?offset=10&limit=10";

        var response = new ListResponse<Person>();
        response.Data = client.People.Take(10);
        response.Links = new ListResponseLinks()
        {
            Next = uri
        };
        response.Meta = new ListResponseMeta()
        {
            CurrentCount = 10
            //Next = new Abstractions.PageInfo(10)
        };

        var result = await Csg.ListQuery.Client.ListResponseExtensions.GetAllPagesAsync<Person>(client, response).ConfigureAwait(false);

        Assert.AreEqual(105, result.DataCount);
        Assert.AreEqual(105, result.Data.Count());
        Assert.AreEqual(11, result.RequestCount);
    }

    [TestMethod]
    public async Task Test_PostAllPagesAsync()
    {
        var client = new MockClient();
            
        for (int i = 1; i <= 105; i++)
        {
            client.People.Add(new Person());
        }

        var response = new ListRequest()
        {
            Offset = 0,
            Limit = 10
        };

        var result = await Csg.ListQuery.Client.ListResponseExtensions.PostAllPagesAsync<Person>(client, "https://example.com/api/people/filter", response).ConfigureAwait(false);

        Assert.AreEqual(105, result.DataCount);
        Assert.AreEqual(105, result.Data.Count());
        Assert.AreEqual(11, result.RequestCount);
    }
}