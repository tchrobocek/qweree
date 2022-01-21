using System;
using Qweree.Utils;

namespace Qweree.TestUtils;

public class StaticDateTimeProvider : IDateTimeProvider
{
    public StaticDateTimeProvider() : this(DateTime.UtcNow)
    {
    }

    public StaticDateTimeProvider(DateTime dateTime)
    {
        UtcNow = dateTime;
    }

    public DateTime UtcNow { get; }
}