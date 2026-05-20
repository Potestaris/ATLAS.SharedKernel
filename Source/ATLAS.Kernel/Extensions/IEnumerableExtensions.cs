using System.ComponentModel;
using System.Data;

namespace ATLAS.Kernel.Extensions;

// ReSharper disable InconsistentNaming
/// <summary>
/// Extension methods for <see cref="IEnumerable{T}"/> collections, providing data conversion utilities.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Converts an enumerable collection of objects to a <see cref="DataTable"/> where each object's properties become columns.
    /// </summary>
    /// <typeparam name="T">The type of objects in the collection.</typeparam>
    /// <param name="data">The enumerable collection to convert. Must not be null.</param>
    /// <returns>A <see cref="DataTable"/> representation of the collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public static DataTable ToDataTable<T>(this IEnumerable<T> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
        DataTable table = new();
        foreach (PropertyDescriptor prop in properties)
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        foreach (var item in data)
        {
            DataRow row = table.NewRow();
            foreach (PropertyDescriptor prop in properties)
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            table.Rows.Add(row);
        }
        return table;
    }
}
// ReSharper restore InconsistentNaming
