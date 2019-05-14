using System;

namespace Samples
{
    public static class Extensions
    {
        public static bool RandBool(this Random r) => r.Next(1) == 1 ? true : false;
    }
}