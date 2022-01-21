using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Qweree.AspNet.Web;

// ReSharper disable once InconsistentNaming
public static class IHeaderDictionaryExtensions
{
    public static void AddLinkHeaders(this IHeaderDictionary headers, string baseUri, int skip, int take,
        long total)
    {
        var headerValues = new List<string>();

        if (!baseUri.Contains("?"))
            baseUri += "?";
        else if (baseUri.Contains("?") && !baseUri.EndsWith("&") && !baseUri.EndsWith("?"))
            baseUri += "&";

        headerValues.Add(@$"<{baseUri}skip=0&take={take}>; rel=""first""");

        //skip = 20, take = 10, total 50 === 21 - 30
        //skip = 10, take = 10, total 50 === 21 - 30
        var prev = skip - take;

        if (prev >= 0)
            headerValues.Add(@$"<{baseUri}skip={prev}&take={take}>; rel=""prev""");

        var next = skip + take;

        if (next < total)
            headerValues.Add(@$"<{baseUri}skip={next}&take={take}>; rel=""next""");

        headerValues.Add(@$"<{baseUri}skip={total - take}&take={take}>; rel=""last""");

        headers.Add("Link", string.Join(", ", headerValues));
    }
}