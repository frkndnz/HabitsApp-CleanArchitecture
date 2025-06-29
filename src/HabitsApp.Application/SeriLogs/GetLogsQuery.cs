using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Logs;
using HabitsApp.Application.SeriLogs.Dtos;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.Application.SeriLogs;
public class GetLogsQuery:IRequest<Result<GetLogsQueryResponse>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class GetLogsQueryResponse
{
    public List<LogDetailDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

internal sealed class GetLogsQueryHandler(ILogRepository logRepository) : IRequestHandler<GetLogsQuery, Result<GetLogsQueryResponse>>
{
    public async Task<Result<GetLogsQueryResponse>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
       var response =await logRepository.GetLogsAsync(request, cancellationToken);

        return Result<GetLogsQueryResponse>.Success(response, "success!");
    }
}
