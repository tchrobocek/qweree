using System;
using System.Collections;
using System.Linq;
using DeepEqual;
using DeepEqual.Syntax;
using Xunit;

namespace Qweree.TestUtils.DeepEqual
{
    public class ImmutableArrayComparison : IComparison
    {
        private readonly bool _withDeepEqual;

        public ImmutableArrayComparison(bool withDeepEqual = true)
        {
            _withDeepEqual = withDeepEqual;
        }

        public bool CanCompare(Type type1, Type type2)
        {
            return (type1.Name == "ImmutableArray`1" && type1.Namespace == "System.Collections.Immutable") ||
                   (type2.Name == "ImmutableArray`1" && type2.Namespace == "System.Collections.Immutable");
        }

        public (ComparisonResult result, IComparisonContext context) Compare(IComparisonContext context, object value1,
            object value2)
        {
            var array1 = ((IEnumerable) value1).Cast<object>().ToArray();
            var array2 = ((IEnumerable) value2).Cast<object>().ToArray();

            if (array1.Length != array2.Length)
            {
                context = context.AddDifference(new BasicDifference(context.Breadcrumb, array1.Length, array2.Length,
                    "Length"));
                return (ComparisonResult.Fail, context);
            }

            if (_withDeepEqual)
            {
                array1.WithDeepEqual(array2)
                    .WithCustomComparison(new MillisecondDateTimeComparison())
                    .Assert();
            }
            else
            {
                Assert.Equal(array1, array2);
            }

            return (ComparisonResult.Pass, context);
        }
    }
}