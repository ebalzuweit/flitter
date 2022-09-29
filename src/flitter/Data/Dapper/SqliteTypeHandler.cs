using System.Data;
using Dapper;

namespace flitter.Data.Dapper;

public abstract class SqliteTypeHandler<T> : SqlMapper.TypeHandler<T>
{
	public override void SetValue(IDbDataParameter parameter, T value) => parameter.Value = value;
}