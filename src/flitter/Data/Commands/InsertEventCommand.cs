using Dapper;
using flitter.Events;
using Microsoft.Data.Sqlite;

namespace flitter.Data.Commands;

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
(Guid, CreatedAt, Type, Data)
VALUES
(:Guid, :CreatedAt, :Type, :Data)";

		var command = new CommandDefinition(InsertStmt, parameters: _event, cancellationToken: cancellationToken);
		await connection.ExecuteAsync(command).ConfigureAwait(false);
	}
}