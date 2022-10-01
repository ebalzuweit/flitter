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

	public string InsertEntityScript()
	{
		var sb = new StringBuilder();
		sb.AppendFormat("INSERT INTO {0} (", _mapping.TableName);

		// TODO: only loop once
		bool firstColumn = true;
		foreach (var column in _mapping.Columns)
		{
			if (column.AutoIncrement)
				continue;

			if (firstColumn)
				firstColumn = false;
			else
				sb.Append(", ");

			sb.Append(column.Name);
		}
		sb.Append(") VALUES (");

		firstColumn = true;
		foreach (var column in _mapping.Columns)
		{
			if (column.AutoIncrement)
				continue;

			if (firstColumn)
				firstColumn = false;
			else
				sb.Append(", ");

			sb.AppendFormat(":{0}", column.Name);
		}
		sb.Append(")");

		return sb.ToString();
	}

	public string SelectEntitiesScript()
	{
		var sb = new StringBuilder();
		sb.AppendFormat("SELECT * FROM {0}", _mapping.TableName);
		return sb.ToString();
	}

	public string SelectEntityByPrimaryKeyScript()
	{
		ValidateMappingHasPrimaryKey();

		var sb = new StringBuilder();
		sb.AppendFormat("SELECT * FROM {0}", _mapping.TableName);
		sb.AppendFormat(" WHERE {0} = :{0}", _mapping.PrimaryKey!.Name);
		return sb.ToString();
	}

	public string UpdateEntityByPrimaryKeyScript()
	{
		ValidateMappingHasPrimaryKey();

		var sb = new StringBuilder();
		sb.AppendFormat("UPDATE {0} SET ", _mapping.TableName);

		var firstColumn = true;
		foreach (var column in _mapping.Columns)
		{
			if (column.IsPrimaryKey)
				continue;
			if (column.AutoIncrement)
				continue;

			if (firstColumn)
				firstColumn = false;
			else
				sb.Append(", ");

			sb.AppendFormat("{0} = :{0}", column.Name);
		}

		sb.AppendFormat(" WHERE {0} = :{0}", _mapping.PrimaryKey!.Name);
		return sb.ToString();
	}

	public string DeleteEntityByPrimaryKeyScript()
	{
		ValidateMappingHasPrimaryKey();

		var sb = new StringBuilder();
		sb.AppendFormat("DELETE FROM {0} WHERE {1} = :{1}", _mapping.TableName, _mapping.PrimaryKey!.Name);
		return sb.ToString();
	}

	private void ValidateMappingHasPrimaryKey()
	{
		if (_mapping.PrimaryKey is null)
			throw new InvalidOperationException("Mapping has no primary key.");
	}
}