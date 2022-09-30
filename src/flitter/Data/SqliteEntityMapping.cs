using System.Reflection;

namespace flitter.Data;

public class SqliteEntityMapping<T>
	where T : new()
{
	public string TableName { get; init; }
	public SqliteColumnMapping[] Columns { get; init; }

	public SqliteEntityMapping()
	{
		TableName = typeof(T).Name;
		Columns = GetColumnMappings().ToArray();
	}

	private IEnumerable<SqliteColumnMapping> GetColumnMappings()
	{
		var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
		return props.Select(propInfo => SqliteColumnMapping.FromPropertyInfo(propInfo));
	}
}