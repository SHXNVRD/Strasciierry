using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;

namespace Strasciierry.Core.Extensions;
public static class ArrayExtensions
{
    public static List<T> ToList<T>(this T[,] array)
    {
        if (array == null)
            return [];

        var y = array.GetLength(0);
        var x = array.GetLength(1);

        var list = new List<T>(x * y);
        
        foreach (var item in array)
        {
            list.Add(item);
        }

        return list;
    }

    public static string Stringify(this char[][] array)
    {
        if (array.Length == 0)
            throw new ArgumentException("The array cannot be of zero length", nameof(array));

        var builder = new StringBuilder();

        foreach (var row in array)
        {
            string cleanRow = new(row.Where(c => !char.IsControl(c)).ToArray());
            builder.AppendLine(cleanRow);
        }

        return builder.ToString();
    }

    public static T[] Reverse<T>(this T[] array)
    {
        var newArray = new T[array.Length];

        array.CopyTo(newArray, 0);
        Array.Reverse(newArray);

        return newArray;
    }
}
