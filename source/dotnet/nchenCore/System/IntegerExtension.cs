using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class IntegerExtension
    {
        public static bool IsEven(this int number) => (number % 2) == 0;
        public static bool IsOdd(this int number) => !number.IsEven();
    }
}
