using Microsoft.Extensions.Options;
using System;

namespace Csg.ListQuery.AspNetCore.Tests.Mock;

public class MockOptionsMonitor<T> : IOptionsMonitor<T>
    where T : class, new()
{
    public MockOptionsMonitor(T currentValue)
    {
        CurrentValue = currentValue;
    }

    public T Get(string name)
    {
        return CurrentValue;
    }

    public IDisposable OnChange(Action<T, string> listener)
    {
        return new MockDisposable();
    }

    public T CurrentValue { get; }
}

public class MockDisposable : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}