using Microsoft.AspNetCore.Http;
using System;

namespace Csg.ListQuery.AspNetCore;

public static class RequestExtensions
{
    public static Uri GetUri(this HttpRequest request)
    {
        return new Uri($"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}");
    }

    public static Uri GetUriWithoutQueryString(this HttpRequest request)
    {
        return new Uri($"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}");
    }

    public static string GetUriWithoutQueryString(this System.Uri uri)
    {
        return $"{uri.Scheme}://{uri.Authority}{uri.AbsolutePath}";
    }
}