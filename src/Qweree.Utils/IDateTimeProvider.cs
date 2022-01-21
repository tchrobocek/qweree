using System;

namespace Qweree.Utils;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}