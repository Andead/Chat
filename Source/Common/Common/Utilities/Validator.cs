using System;
using System.Collections.Generic;
using System.Linq;

namespace Andead.Chat.Common.Utilities
{
    public static class Validator
    {
        public static void IsNotNull(this object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void IsIn(this object value, params object[] range)
        {
            if (!range.Contains(value))
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}