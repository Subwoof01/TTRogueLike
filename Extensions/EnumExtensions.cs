using System;
using System.Text.RegularExpressions;

namespace RogueLike.Extensions
{
    internal static class EnumExtensions
    {
        public static string ToDescription(this Enum value)
        {
            return string.Join(" ", Regex.Split(value.ToString(), @"(?<!^)(?=[A-Z])"));
        }
    }
}
