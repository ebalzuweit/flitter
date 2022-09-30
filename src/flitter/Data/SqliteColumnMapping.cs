using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace flitter.Data;

public class SqliteColumnMapping
{
	public string Name { get; init; }
	public Type Type { get; init; }
	public string SqliteType { get; init; }
	public bool NotNull { get; init; }
	public bool IsPrimaryKey { get; init; }
	public bool AutoIncrement { get; init; }

	public SqliteColumnMapping(string name, Type type, bool notNull = false, bool isPrimaryKey = false, bool autoIncrement = false)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentNullException(nameof(name));
		if (autoIncrement && IntegralNumericTypes.Contains(type) == false)
			throw new ArgumentException("Auto-increment requires an integral, numeric type.");

		Name = name;
		Type = type ?? throw new ArgumentNullException(nameof(type));
		SqliteType = GetSqliteColumnType(type);
		NotNull = notNull;
		IsPrimaryKey = isPrimaryKey;
		AutoIncrement = autoIncrement;
	}

	public static SqliteColumnMapping FromPropertyInfo(PropertyInfo propertyInfo)
	{
		var requiredAttr = propertyInfo.GetCustomAttribute<RequiredAttribute>();
		var keyAttr = propertyInfo.GetCustomAttribute<KeyAttribute>();
		var incrementAttr = propertyInfo.GetCustomAttribute<AutoIncrementAttribute>();

		return new SqliteColumnMapping(
			name: propertyInfo.Name,
			type: propertyInfo.PropertyType,
			notNull: requiredAttr is not null,
			isPrimaryKey: keyAttr is not null,
			autoIncrement: incrementAttr is not null
		);
	}

	private string GetSqliteColumnType(Type type)
	{
		if (IntegralNumericTypes.Contains(type))
			return "INTEGER";
		if (FloatingPointNumericTypes.Contains(type))
			return "REAL";
		if (type == typeof(byte[]))
			throw new NotSupportedException("BLOB data type is not supported yet.");

		// default to text, custom SqliteTypeHandler can be used for all other types
		return "TEXT";
	}

	// dotnet 7 gives us the INumber interface, which can replace these in the future.
	private static readonly HashSet<Type> IntegralNumericTypes = new HashSet<Type>
	{
		typeof(sbyte), typeof(byte), typeof(short), typeof(ushort),
		typeof(int), typeof(uint), typeof(long), typeof(ulong),
		typeof(nint), typeof(nuint),
		typeof(sbyte?), typeof(byte?), typeof(short?), typeof(ushort?),
		typeof(int?), typeof(uint?), typeof(long?), typeof(ulong?),
		typeof(nint?), typeof(nuint?)
	};

	private static readonly HashSet<Type> FloatingPointNumericTypes = new HashSet<Type>
	{
		typeof(float), typeof(double), typeof(decimal),
		typeof(float?), typeof(double?), typeof(decimal?)
	};
}