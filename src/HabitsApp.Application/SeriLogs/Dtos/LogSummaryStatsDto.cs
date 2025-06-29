using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitsApp.Application.Logs.Dtos;
public class LogSummaryStatsDto
{
    public int TotalErrorCount { get; set; }
    public int TodayErrorCount { get; set; }
    public string MostFailingEndpoint { get; set; } = default!;
    public string MostCommonException { get; set; } = default!;
    public Dictionary<string, int> ErrorLevelCounts { get; set; } = new();
    public string MostFailingUser { get; set; } = default!;
}
