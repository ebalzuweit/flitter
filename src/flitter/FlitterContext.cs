using System.Collections.Concurrent;
using Dapper;
using flitter.Data.Dapper;
using Microsoft.Data.Sqlite;

namespace flitter;

public class FlitterContext : IDisposable, IAsyncDisposable
{
	public const string InMemorySharedConnectionString = "Data Source=flitter;Mode=Memory;Cache=Shared";

	private readonly string _connectionString;
	private readonly ConcurrentDictionary<WeakReference<SqliteConnection>, bool> _connections = new();
	private readonly SqliteConnection? _persistentConnection;

	private bool _isDisposed = false;

	public FlitterContext() : this(InMemorySharedConnectionString, inMemory: true) { }

	public FlitterContext(string connectionString, bool inMemory = false)
	{
		_connectionString = connectionString;

		SqlMapper.AddTypeHandler(new GuidHandler());

		if (inMemory)
		{
			// persist a connection to keep the shared in-memory database alive
			// (https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/in-memory-databases#shareable-in-memory-databases)
			_persistentConnection = CreateConnectionInternal();
			_persistentConnection.Open();
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public async ValueTask DisposeAsync()
	{
		await DisposeCoreAsync().ConfigureAwait(false);
		GC.SuppressFinalize(this);
	}

	public async Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
	{
		await using var _ = CreateConnection(out var connection).ConfigureAwait(false);
		var result = await command.ExecuteAsync(connection, cancellationToken).ConfigureAwait(false);
		return result;
	}

	public async Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default)
	{
		await using var _ = CreateConnection(out var connection).ConfigureAwait(false);
		await command.ExecuteAsync(connection, cancellationToken).ConfigureAwait(false);
	}

	private IAsyncDisposable CreateConnection(out SqliteConnection connection)
	{
		connection = CreateConnectionInternal();
		return connection;
	}

	private SqliteConnection CreateConnectionInternal()
	{
		var connection = new SqliteConnection(_connectionString);
		var weakRef = new WeakReference<SqliteConnection>(connection);
		_connections.TryAdd(weakRef, default);
		return connection;
	}

	private void Dispose(bool disposing)
	{
		if (_isDisposed) return;
		if (disposing)
		{
			DisposeCoreAsync().AsTask().GetAwaiter().GetResult();
		}
		_isDisposed = true;
	}

	private async ValueTask DisposeCoreAsync()
	{
		if (_isDisposed) return;

		// cleanup connections
		foreach (var weakRef in _connections.Keys)
		{
			if (weakRef.TryGetTarget(out var connection))
			{
				try { await connection.DisposeAsync().ConfigureAwait(false); }
				catch (ObjectDisposedException) { }
			}
		}
		_connections.Clear();

		_persistentConnection?.Dispose();

		_isDisposed = true;
	}
}