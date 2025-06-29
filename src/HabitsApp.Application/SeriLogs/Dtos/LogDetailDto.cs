using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitsApp.Application.SeriLogs.Dtos;
public sealed class LogDetailDto
{
    
    public DateTime TimeStamp { get; set; }
    public string Level { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? Exception { get; set; }
    public string? Path { get; set; }
    public string? Method { get; set; }
    public string? User { get; set; }
}
