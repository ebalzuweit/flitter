using System.Collections.Concurrent;
using Dapper;
using flitter.Data.Dapper;
using Microsoft.Data.Sqlite;

namespace flitter.Data;

public class FlitterContext : IDisposable, IAsyncDisposable
{
	public const string InMemorySharedConnectionString = "Data Source=flitter;Mode=Memory;Cache=Shared";

	private readonly string _connectionString;
	private readonly ConcurrentDictionary<WeakReference<SqliteConnection>, bool> _connections = new();
	private readonly SqliteConnection? _persistentConnection;
	private readonly ConcurrentDictionary<Type, SqliteQueryBuilder> _queryBuilders = new();

	private bool _isDisposed = false;

	public FlitterContext() : this(InMemorySharedConnectionString, inMemory: true) { }

	public FlitterContext(string connectionString, bool inMemory = false)
	{
		_connectionString = connectionString;

		SqlMapper.AddTypeHandler(new GuidHandler());
		SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());

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

	public bool RegisterEntity<T>()
		where T : new()
	{
		var type = typeof(T);
		var builder = new SqliteQueryBuilder(type);
		return _queryBuilders.TryAdd(type, builder);
	}

	public async Task Save(string filepath = "flitter.db", CancellationToken cancellationToken = default)
	{
		var connectionString = $"Data Source={filepath}";
		await using var backup = new SqliteConnection(connectionString);
		await backup.OpenAsync();

		await using var _ = CreateConnection(out var connection).ConfigureAwait(false);
		await connection.OpenAsync();
		connection.BackupDatabase(backup);
	}

	public async Task CreateTableAsync<T>(CancellationToken cancellationToken = default)
		where T : new()
	{
		var builder = GetQueryBuilderOrThrow<T>();
		var script = builder.CreateTableScript();
		var command = new CommandDefinition(script, cancellationToken: cancellationToken);

		await using var _ = CreateConnection(out var connection).ConfigureAwait(false);
		await connection.ExecuteAsync(command).ConfigureAwait(false);
	}

	public async Task<bool> InsertEntityAsync<T>(T entity, CancellationToken cancellationToken = default)
		where T : new()
	{
		var builder = GetQueryBuilderOrThrow<T>();
		var script = builder.InsertEntityScript();
		var command = new CommandDefinition(script, parameters: entity, cancellationToken: cancellationToken);

		await using var _ = CreateConnection(out var connection).ConfigureAwait(false);
		var rowsAffected = await connection.ExecuteAsync(command).ConfigureAwait(false);
		return rowsAffected > 0;
	}

	public async Task<IEnumerable<T>> GetEntitiesAsync<T>(CancellationToken cancellationToken = default)
		where T : new()
	{
		var builder = GetQueryBuilderOrThrow<T>();
		var script = builder.SelectEntitiesScript();
		var command = new CommandDefinition(script, cancellationToken: cancellationToken);

		await using var _ = CreateConnection(out var connection).ConfigureAwait(false);
		var entities = await connection.QueryAsync<T>(command).ConfigureAwait(false);
		return entities;
	}

	public async Task<T> GetEntityAsync<T>(T entity, CancellationToken cancellationToken = default)
		where T : new()
	{
		var builder = GetQueryBuilderOrThrow<T>();
		var script = builder.SelectEntityByPrimaryKeyScript();
		var command = new CommandDefinition(script, parameters: entity, cancellationToken: cancellationToken);

		await using var _ = CreateConnection(out var connection).ConfigureAwait(false);
		var e = await connection.QuerySingleAsync<T>(command).ConfigureAwait(false);
		return e;
	}

	public async Task<bool> UpdateEntityAsync<T>(T entity, CancellationToken cancellationToken = default)
		where T : new()
	{
		var builder = GetQueryBuilderOrThrow<T>();
		var script = builder.UpdateEntityByPrimaryKeyScript();
		var command = new CommandDefinition(script, parameters: entity, cancellationToken: cancellationToken);

		await using var _ = CreateConnection(out var connection).ConfigureAwait(false);
		var rowsAffected = await connection.ExecuteAsync(command);
		return rowsAffected > 0;
	}

	public async Task<bool> DeleteEntityAsync<T>(T entity, CancellationToken cancellationToken = default)
		where T : new()
	{
		var builder = GetQueryBuilderOrThrow<T>();
		var script = builder.DeleteEntityByPrimaryKeyScript();
		var command = new CommandDefinition(script, parameters: entity, cancellationToken: cancellationToken);

		await using var _ = CreateConnection(out var connection).ConfigureAwait(false);
		var rowsAffected = await connection.ExecuteAsync(command);
		return rowsAffected > 0;
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

	private SqliteQueryBuilder GetQueryBuilderOrThrow<T>()
		where T : new()
	{
		var type = typeof(T);
		if (_queryBuilders.TryGetValue(type, out var builder))
			return builder;

		throw new FlitterException("Failed to get query builder for type. Did you register the entity?");
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