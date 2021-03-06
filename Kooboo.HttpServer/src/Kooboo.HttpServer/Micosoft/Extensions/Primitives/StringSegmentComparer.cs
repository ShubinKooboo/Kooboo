﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Primitives
{
    public class StringSegmentComparer : IEqualityComparer<StringSegment>
    {
        public static StringSegmentComparer Ordinal { get; }
            = new StringSegmentComparer(StringComparison.Ordinal, StringComparer.Ordinal);
        public static StringSegmentComparer OrdinalIgnoreCase { get; }
            = new StringSegmentComparer(StringComparison.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);

        private StringSegmentComparer(StringComparison comparison, StringComparer comparer)
        {
            Comparison = comparison;
            Comparer = comparer;
        }

        private StringComparison Comparison { get; }
        private StringComparer Comparer { get; }

        public bool Equals(StringSegment x, StringSegment y)
        {
            return StringSegment.Equals(x, y, Comparison);
        }

        public int GetHashCode(StringSegment obj)
        {
            // TODO: PERF; Note that .NET Core strings use randomized hash codes for security reasons.
            return Comparer.GetHashCode(obj.Value);
        }
    }
}
