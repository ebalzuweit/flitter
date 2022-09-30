using Microsoft.Data.Sqlite;

namespace flitter.Data;

public interface ICommand<TResult>
{
	Task<TResult> ExecuteAsync(SqliteConnection connection, CancellationToken cancellationToken = default);
}

public interface ICommand
{
	Task ExecuteAsync(SqliteConnection connection, CancellationToken cancellationToken = default);
}