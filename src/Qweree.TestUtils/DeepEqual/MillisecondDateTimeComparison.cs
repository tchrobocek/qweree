using System;
using DeepEqual;

namespace Qweree.TestUtils.DeepEqual
{
    public class MillisecondDateTimeComparison : IComparison
    {
        private readonly int _tolerance;

        public MillisecondDateTimeComparison(int tolerance = 1)
        {
            _tolerance = tolerance;
        }

        public bool CanCompare(Type type1, Type type2)
        {
            return type1 == typeof(DateTime) && type2 == typeof(DateTime);
        }

        public (ComparisonResult result, IComparisonContext context) Compare(IComparisonContext context, object value1,
            object value2)
        {
            var dateTime1 = (DateTime) value1;
            var dateTime2 = (DateTime) value2;

            if ((dateTime1 - dateTime2).TotalMilliseconds > _tolerance)
            {
                context = context.AddDifference(dateTime1, dateTime2);
                return (ComparisonResult.Fail, context);
            }

            return (ComparisonResult.Pass, context);
        }
    }
}