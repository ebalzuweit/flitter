namespace flitter.Data.Dapper;

internal class DateTimeOffsetHandler : SqliteTypeHandler<DateTimeOffset>
{
	public override DateTimeOffset Parse(object value) => DateTimeOffset.Parse((string)value);
}