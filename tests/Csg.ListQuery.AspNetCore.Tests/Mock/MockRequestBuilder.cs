using System;
using System.Threading.Tasks;
using Csg.ListQuery.Server;

namespace Csg.ListQuery.AspNetCore.Tests.Mock;

public class MockRequestBuilder<T> : Csg.ListQuery.Client.ListRequestBuilder<T>
{
    public override Task<IListResponse<T>> GetResponseAsync()
    {
        throw new NotImplementedException();
    }
}