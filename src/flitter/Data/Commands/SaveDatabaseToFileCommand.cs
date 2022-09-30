using Microsoft.Data.Sqlite;

namespace flitter.Data.Commands;

public class SaveDatabaseToFileCommand : ICommand<bool>
{
	private readonly string _filepath;

	public SaveDatabaseToFileCommand() : this(null) { }

	public SaveDatabaseToFileCommand(string? filepath)
	{
		// TODO: alphanumeric check on filepath
		_filepath = string.IsNullOrWhiteSpace(filepath) ? "flitter.db" : filepath;
	}

	public async Task<bool> ExecuteAsync(SqliteConnection connection, CancellationToken cancellationToken)
	{
		await using var backup = new SqliteConnection($"Data Source={_filepath}");
		await backup.OpenAsync();

		await connection.OpenAsync();
		connection.BackupDatabase(backup);

		return true;
	}
}
