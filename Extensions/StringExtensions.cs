using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Extensions
{
    public static class StringExtensions
    {
        public static string RemovePostFix(this string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }
            else
            {
                return s;
            }
        }
    }
}
