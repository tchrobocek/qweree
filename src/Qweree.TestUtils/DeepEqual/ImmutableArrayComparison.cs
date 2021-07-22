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
        private readonly IComparison? _inner;
        private readonly bool _withDeepEqual;

        public ImmutableArrayComparison(IComparison? inner = null, bool withDeepEqual = true)
        {
            _inner = inner;
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
            if (value1 is not IEnumerable enumerable1 || value2 is not IEnumerable enumerable2)
            {
                context = context.AddDifference(new BasicDifference(context.Breadcrumb, value1.GetType(), value2.GetType(), ""));
                return (ComparisonResult.Fail, context);
            }

            var array1 = enumerable1.Cast<object>().ToArray();
            var array2 = enumerable2.Cast<object>().ToArray();

            if (array1.Length != array2.Length)
            {
                context = context.AddDifference(new BasicDifference(context.Breadcrumb, array1.Length, array2.Length,
                    "Length"));
                return (ComparisonResult.Fail, context);
            }

            if (_withDeepEqual)
            {
                var syntax = array1.WithDeepEqual(array2);
                if (_inner != null)
                {
                    syntax = syntax.WithCustomComparison(_inner);
                }
                syntax.Assert();
            }
            else
            {
                Assert.Equal(array1, array2);
            }

            return (ComparisonResult.Pass, context);
        }
    }
}