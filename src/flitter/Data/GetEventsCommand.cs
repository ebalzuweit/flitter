using Dapper;
using flitter.Events;
using Microsoft.Data.Sqlite;

namespace flitter.Data;

public class GetEventsCommand : ICommand<IEnumerable<Event>>
{
	public async Task<IEnumerable<Event>> ExecuteAsync(SqliteConnection connection, CancellationToken cancellationToken = default)
	{
		const string EventsQuery = "SELECT * FROM Events";
		var command = new CommandDefinition(EventsQuery, cancellationToken: cancellationToken);
		var events = await connection.QueryAsync<Event>(command).ConfigureAwait(false);
		return events;
	}
}