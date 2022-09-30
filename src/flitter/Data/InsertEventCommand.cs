using Dapper;
using flitter.Events;
using Microsoft.Data.Sqlite;

namespace flitter.Data;

public class InsertEventCommand : ICommand
{
	private readonly Event _event;

	public InsertEventCommand(Event @event)
	{
		_event = @event ?? throw new System.ArgumentNullException(nameof(@event));
	}

	public async Task ExecuteAsync(SqliteConnection connection, CancellationToken cancellationToken = default)
	{
		const string InsertStmt = @"INSERT INTO Events
(Guid, CreatedAt)
VALUES
(:Guid, :CreatedAt)";

		var command = new CommandDefinition(InsertStmt, parameters: _event, cancellationToken: cancellationToken);
		await connection.ExecuteAsync(command).ConfigureAwait(false);
	}
}