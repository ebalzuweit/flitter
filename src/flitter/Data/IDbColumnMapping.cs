using System.Reflection;

namespace flitter.Data;

public interface IDbColumnMapping
{
	string Name { get; }
	Type Type { get; }
	string DbType { get; }
	bool NotNull { get; }
	bool IsPrimaryKey { get; }
	bool AutoIncrement { get; }

	static IDbColumnMapping FromPropertyInfo(PropertyInfo propertyInfo)
		=> throw new NotImplementedException();
}