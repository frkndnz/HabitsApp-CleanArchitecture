using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Logs;
using HabitsApp.Application.Logs.Dtos;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.Application.SeriLogs;
public record GetLogSummaryStatsQuery() : IRequest<Result<LogSummaryStatsDto>>;


internal sealed class GetLogSummaryStatsQueryHandler(
    ILogRepository logRepository
    ) : IRequestHandler<GetLogSummaryStatsQuery, Result<LogSummaryStatsDto>>
{
    public async Task<Result<LogSummaryStatsDto>> Handle(GetLogSummaryStatsQuery request, CancellationToken cancellationToken)
    {
        var result = await logRepository.GetLogSummaryStatsAsync(cancellationToken);

        return Result<LogSummaryStatsDto>.Success(result, "success!");
    }
}
