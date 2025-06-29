using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Logs;
using HabitsApp.Application.Logs.Dtos;
using HabitsApp.Application.SeriLogs;
using HabitsApp.Application.SeriLogs.Dtos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HabitsApp.Infrastructure.Services;
internal class LogRepository : ILogRepository
{
    private readonly IConfiguration _configuration;

    public LogRepository(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public async Task<GetLogsQueryResponse> GetLogsAsync(GetLogsQuery query, CancellationToken cancellationToken = default)
    {
        var result = new GetLogsQueryResponse
        {
            Page = query.Page,
            PageSize = query.PageSize
        };

        var connStr = _configuration.GetConnectionString("SqlServer")!;
        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync(cancellationToken);

        // 1. Toplam kayıt sayısı
        const string countSql = "SELECT COUNT(*) FROM ErrorLogs";
        await using (var countCmd = new SqlCommand(countSql, conn))
        {
            result.TotalCount = (int)await countCmd.ExecuteScalarAsync(cancellationToken);
        }

        // 2. Sayfalı veriyi çek
        const string pagedSql = @"
        WITH LogsPaged AS (
            SELECT
                ROW_NUMBER() OVER (ORDER BY TimeStamp DESC) AS RowNum,
                TimeStamp,
                Level,
                Message,
                Exception,
                CAST(Properties AS XML).value('(/properties/property[@key=""Path""])[1]', 'nvarchar(500)') AS Path,
                CAST(Properties AS XML).value('(/properties/property[@key=""Method""])[1]', 'nvarchar(50)') AS Method,
                CAST(Properties AS XML).value('(/properties/property[@key=""User""])[1]', 'nvarchar(200)') AS [User]
            FROM ErrorLogs
        )
        SELECT *
        FROM LogsPaged
        WHERE RowNum BETWEEN ((@PageNumber - 1) * @PageSize + 1) AND (@PageNumber * @PageSize)
        ORDER BY TimeStamp DESC";

        await using (var cmd = new SqlCommand(pagedSql, conn))
        {
            cmd.Parameters.AddWithValue("@PageNumber", query.Page);
            cmd.Parameters.AddWithValue("@PageSize", query.PageSize);

            var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var dto = new LogDetailDto
                {
                    TimeStamp = reader.GetDateTime(reader.GetOrdinal("TimeStamp")),
                    Level = reader.GetString(reader.GetOrdinal("Level")),
                    Message = reader.GetString(reader.GetOrdinal("Message")),
                    Exception = reader.IsDBNull(reader.GetOrdinal("Exception")) ? null : reader.GetString(reader.GetOrdinal("Exception")),
                    Path = reader.IsDBNull(reader.GetOrdinal("Path")) ? null : reader.GetString(reader.GetOrdinal("Path")),
                    Method = reader.IsDBNull(reader.GetOrdinal("Method")) ? null : reader.GetString(reader.GetOrdinal("Method")),
                    User = reader.IsDBNull(reader.GetOrdinal("User")) ? null : reader.GetString(reader.GetOrdinal("User")),
                };

                result.Items.Add(dto);
            }

            await reader.DisposeAsync();
        }

        return result;
    }

    public async Task<LogSummaryStatsDto> GetLogSummaryStatsAsync(CancellationToken cancellationToken = default)
    {
        var result = new LogSummaryStatsDto();
        var connStr = _configuration.GetConnectionString("SqlServer")!;

        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync(cancellationToken);

        // 1. Total error count
        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM ErrorLogs", conn))
        {
            result.TotalErrorCount = (int)await cmd.ExecuteScalarAsync(cancellationToken);
        }

        // 2. Today error count
        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM ErrorLogs WHERE CAST(TimeStamp AS DATE) = CAST(GETDATE() AS DATE)", conn))
        {
            result.TodayErrorCount = (int)await cmd.ExecuteScalarAsync(cancellationToken);
        }

        // 3. Most failing endpoint
        using (var cmd = new SqlCommand(@"
            SELECT TOP 1 x.Path, COUNT(*) AS Count
            FROM ErrorLogs e
            CROSS APPLY (
                SELECT CAST(e.Properties AS XML).value('(/properties/property[@key=""Path""])[1]', 'nvarchar(500)') AS Path
            ) AS x
            GROUP BY x.Path
            ORDER BY Count DESC", conn))
        {
            var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
                result.MostFailingEndpoint = reader.GetString(0);
            await reader.DisposeAsync();
        }

        // 4. Most common exception type
        using (var cmd = new SqlCommand(@"
            SELECT TOP 1 
                LEFT(Exception, CHARINDEX(':', Exception + ':') - 1) AS ExceptionType,
                COUNT(*) AS Count
            FROM ErrorLogs
            WHERE Exception IS NOT NULL
            GROUP BY LEFT(Exception, CHARINDEX(':', Exception + ':') - 1)
            ORDER BY Count DESC", conn))
        {
            var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
                result.MostCommonException = reader.GetString(0);
            await reader.DisposeAsync();
        }

        // 5. Error level counts
        using (var cmd = new SqlCommand("SELECT Level, COUNT(*) FROM ErrorLogs GROUP BY Level", conn))
        {
            var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var level = reader.GetString(0);
                var count = reader.GetInt32(1);
                result.ErrorLevelCounts[level] = count;
            }
            await reader.DisposeAsync();
        }

        // 6. Most failing user
        using (var cmd = new SqlCommand(@"
            SELECT TOP 1 x.UserId, COUNT(*) AS Count
            FROM ErrorLogs e
            CROSS APPLY (
                SELECT CAST(e.Properties AS XML).value('(/properties/property[@key=""User""])[1]', 'nvarchar(200)') AS UserId
            ) AS x
            GROUP BY x.UserId
            ORDER BY Count DESC", conn))
        {
            var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
                result.MostFailingUser = reader.GetString(0);
            await reader.DisposeAsync();
        }

        return result;
    }

   
}
