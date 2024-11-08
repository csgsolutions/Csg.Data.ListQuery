using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Csg.ListQuery.Server;
using Csg.ListQuery.AspNetCore.ModelBinding;

namespace Csg.ListQuery.AspNetCore.Tests;

public class MockClient : Csg.ListQuery.Client.IPagedListSupport
{
    public MockClient()
    {
    }

    public IList<Person> People { get; set; } = new List<Person>();
                
    public Task<IListResponse<TData>> GetAsync<TData>(string url)
    {
        var factory = new ListRequestFactory();
        var uri = new System.Uri(url);
        var request = factory.CreateRequest<ListRequest>(uri.Query);
            
        IEnumerable<TData> dataSource = null;
        if (typeof(TData) == typeof(Person))
        {
            dataSource = this.People.Cast<TData>();
        }
        else
        {
            throw new NotSupportedException();
        }

        var properties = PropertyHelper.GetProperties(typeof(TData));

        var resultData = dataSource.Skip(request.Offset.Value).Take(request.Limit.Value);
        var queryResult = new Csg.ListQuery.ListQueryResult<TData>(
            resultData,
            resultData.Count(),
            dataSource.Count(),
            true,
            request.Limit,
            (request.Offset + resultData.Count()) < dataSource.Count() ? request.Offset + request.Limit : (int?)null,
            request.Offset > 0 ? Math.Max(request.Offset.Value - request.Limit.Value, 0) : (int?)null
        );

        var currentUri = new System.Uri(url);

        return Task.FromResult<IListResponse<TData>>(
            Csg.ListQuery.AspNetCore.ListQueryResultExtensions.ToListResponse<TData, TData>(queryResult, request, properties, x=>x, currentUri)
        );
    }

    public Task<IListResponse<TData>> PostAsync<TData>(string url, IListRequest request)
    {
        IEnumerable<TData> dataSource = null;
        if (typeof(TData) == typeof(Person))
        {
            dataSource = this.People.Cast<TData>();
        }
        else
        {
            throw new NotSupportedException();
        }

        var properties = PropertyHelper.GetProperties(typeof(TData));

        var resultData = dataSource.Skip(request.Offset.Value).Take(request.Limit.Value);

        var queryResult = new Csg.ListQuery.ListQueryResult<TData>(
            resultData,
            resultData.Count(),
            dataSource.Count(),
            true,
            request.Limit,
            (request.Offset + resultData.Count()) < dataSource.Count() ? request.Offset + request.Limit : (int?)null,
            request.Offset > 0 ? Math.Max(request.Offset.Value - request.Limit.Value, 0) :(int?)null
        );

        return Task.FromResult<IListResponse<TData>>(
            Csg.ListQuery.AspNetCore.ListQueryResultExtensions.ToListResponse<TData, TData>(queryResult, request, properties, x => x)
        );
    }
}