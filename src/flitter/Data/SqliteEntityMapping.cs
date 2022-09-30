using System.Reflection;

namespace flitter.Data;

public class SqliteEntityMapping
{
	public string TableName { get; init; }
	public SqliteColumnMapping[] Columns { get; init; }

	public SqliteEntityMapping(Type type)
	{
		if (type.IsValueType)
			throw new ArgumentException("Type must be a reference type.");
		if (type.IsAbstract)
			throw new ArgumentException("Type must be concrete.");
		if (type.GetConstructor(Type.EmptyTypes) is null)
			throw new ArgumentException("Type must have a public, empty constructor.");

		TableName = type.Name;
		Columns = GetColumnMappings(type).ToArray();
	}

	private IEnumerable<SqliteColumnMapping> GetColumnMappings(Type type)
	{
		var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
		return props.Select(propInfo => SqliteColumnMapping.FromPropertyInfo(propInfo));
	}
}