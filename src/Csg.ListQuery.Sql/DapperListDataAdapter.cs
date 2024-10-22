using Csg.Data;
using Csg.Data.Sql;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Csg.ListQuery.Sql;

public class DapperListDataAdapter : IListQueryDataAdapter
{
    private System.Data.IDbConnection _connection;
    private System.Data.IDbTransaction _transaction;

    public DapperListDataAdapter(System.Data.IDbConnection connection, System.Data.IDbTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<IEnumerable<T>> GetResultsAsync<T>(SqlStatementBatch batch, bool stream, int commandTimeout, CancellationToken cancellationToken = default)
    {
        var cmdFlags = stream ? Dapper.CommandFlags.Pipelined : Dapper.CommandFlags.Buffered;
        var cmd = batch.ToDapperCommand(_transaction, commandTimeout, commandFlags: cmdFlags, cancellationToken);

        return await Dapper.SqlMapper.QueryAsync<T>(_connection, cmd).ConfigureAwait(false);
    }

    public async Task<BatchResult<T>> GetTotalCountAndResultsAsync<T>(SqlStatementBatch batch, bool stream, int commandTimeout, CancellationToken cancellationToken = default)
    {
        var cmdFlags = stream ? Dapper.CommandFlags.Pipelined : Dapper.CommandFlags.Buffered;
        var cmd = batch.ToDapperCommand(_transaction, commandTimeout, commandFlags: cmdFlags, cancellationToken);

        var result = new BatchResult<T>();

        using (var batchReader = await Dapper.SqlMapper.QueryMultipleAsync(_connection, cmd).ConfigureAwait(false))
        {
            result.TotalCount = await batchReader.ReadFirstAsync<int>().ConfigureAwait(false);
            result.Items = await batchReader.ReadAsync<T>().ConfigureAwait(false);
        }

        return result;
    }
}