using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strasciierry.Core.Extensions;
public static class CharExtensions
{
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
}
