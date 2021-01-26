using System;
using System.Collections.Generic;
using System.Linq;
using DeepEqual;
using DeepEqual.Syntax;

namespace Qweree.TestUtils.DeepEqual
{
    public class ImmutableArrayComparison : IComparison
    {
        public bool CanCompare(Type type1, Type type2)
        {
            if (typeof(IEnumerable<object>).IsAssignableFrom(type1) &&
                typeof(IEnumerable<object>).IsAssignableFrom(type2))
                if (type1.Name == "ImmutableArray`1" || type2.Name == "ImmutableArray`1")
                    return true;

            return false;
        }

        public (ComparisonResult result, IComparisonContext context) Compare(IComparisonContext context, object value1,
            object value2)
        {
            var array1 = ((IEnumerable<object>) value1).ToArray();
            var array2 = ((IEnumerable<object>) value2).ToArray();

            if (array1.Length != array2.Length)
            {
                context = context.AddDifference(new BasicDifference(context.Breadcrumb, array1.Length, array2.Length,
                    "Length"));
                return (ComparisonResult.Fail, context);
            }

            for (var i = 0; i < array1.Length; i++)
            {
                var val1 = array1[i];
                var val2 = array2[i];
                val1.ShouldDeepEqual(val2);
            }

            return (ComparisonResult.Pass, context);
        }
    }
}