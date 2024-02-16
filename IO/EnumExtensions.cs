using System;
using System.Collections.Generic;
using System.Linq;

namespace GotaSoundIO.IO;

/// <summary>
///     Represents a collection of methods extending enum types. From Syroot.BinaryData.
/// </summary>
internal static class EnumExtensions
{
    private static readonly Dictionary<Type, bool> _flagEnums = new();

    /// <summary>
    ///     Returns whether <paramref name="value" /> is a defined value in the enum of the given <paramref name="type" />
    ///     or a valid set of flags for enums decorated with the <see cref="T:System.FlagsAttribute" />.
    /// </summary>
    /// <param name="type">The type of the enum.</param>
    /// <param name="value">The value to check against the enum type.</param>
    /// <returns><c>true</c> if the value is valid; otherwise <c>false</c>.</returns>
    internal static bool IsValid(Type type, object value)
    {
        var flag = Enum.IsDefined(type, value);
        if (!flag && IsFlagsEnum(type))
        {
            long num = 0;
            foreach (var obj in Enum.GetValues(type))
                num |= Convert.ToInt64(obj);
            var int64 = Convert.ToInt64(value);
            flag = (num & int64) == int64;
        }

        return flag;
    }

    private static bool IsFlagsEnum(Type type)
    {
        bool flag;
        if (!_flagEnums.TryGetValue(type, out flag))
        {
            var customAttributes = type.GetCustomAttributes(typeof(FlagsAttribute), true);
            flag = customAttributes != null && customAttributes.Any();
            _flagEnums.Add(type, flag);
        }

        return flag;
    }
}