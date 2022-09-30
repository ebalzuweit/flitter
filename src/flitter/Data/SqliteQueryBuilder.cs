using System.Text;

namespace flitter.Data;

public class SqliteQueryBuilder<T>
	where T : new()
{
	private readonly SqliteEntityMapping _mapping;

	public SqliteQueryBuilder()
	{
		_mapping = new SqliteEntityMapping(typeof(T));
	}

	public string CreateTableScript()
	{
		var type = typeof(T);

		var sb = new StringBuilder();
		sb.AppendFormat("CREATE TABLE IF NOT EXISTS {0} (", _mapping.TableName);

		for (int i = 0; i < _mapping.Columns.Length; i++)
		{
			var column = _mapping.Columns[i];

			if (i > 0)
				sb.Append(", ");

			sb.AppendFormat("{0} {1}", column.Name, column.SqliteType);
			if (column.NotNull)
				sb.Append(" NOT NULL");
			if (column.IsPrimaryKey)
				sb.Append(" PRIMARY KEY");
			if (column.AutoIncrement)
				sb.Append(" AUTOINCREMENT");
		}

		sb.Append(")");

		return sb.ToString();
	}
}