using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Logs.Dtos;
using HabitsApp.Application.SeriLogs;
using HabitsApp.Application.SeriLogs.Dtos;

namespace HabitsApp.Application.Logs;
public interface ILogRepository
{
    Task<LogSummaryStatsDto> GetLogSummaryStatsAsync(CancellationToken cancellationToken = default);

    Task<GetLogsQueryResponse> GetLogsAsync(GetLogsQuery query,CancellationToken cancellationToken = default);
}
