using System.Data.Common;
using Dapper;
using flitter.Events;
using Microsoft.Data.Sqlite;

namespace flitter.Data;

public class CreateDatabaseCommand : ICommand<bool>
{
	public async Task<bool> ExecuteAsync(SqliteConnection connection, CancellationToken cancellationToken)
	{
		await EnableWriteAheadLogging(connection, cancellationToken);

		await CreateEventsTable(connection, cancellationToken).ConfigureAwait(false);
		var events = await SelectEventsAsync(connection, cancellationToken).ConfigureAwait(false);

		return true;
	}

	public async Task EnableWriteAheadLogging(SqliteConnection connection, CancellationToken cancellationToken)
	{
		await connection.ExecuteAsync("PRAGMA journal_mode = 'wal'").ConfigureAwait(false);
	}

	public async Task CreateEventsTable(DbConnection connection, CancellationToken cancellationToken)
	{
		const string CreateEventsTableStmt = @"CREATE TABLE Events (
	Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	Guid TEXT NOT NULL,
	CreatedAt TEXT NOT NULL,
	EventType TEXT,
	EventMessage TEXT
)";

		var command = new CommandDefinition(CreateEventsTableStmt, cancellationToken: cancellationToken);
		_ = await connection.ExecuteAsync(command).ConfigureAwait(false);
	}

	public async Task<IEnumerable<Event>> SelectEventsAsync(DbConnection connection, CancellationToken cancellationToken)
	{
		const string SelectEventsQuery = @"SELECT * FROM Events";

		var command = new CommandDefinition(SelectEventsQuery, cancellationToken: cancellationToken);
		var result = await connection.QueryAsync<Event>(command).ConfigureAwait(false);
		return result;
	}
}