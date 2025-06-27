using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions;

namespace HabitsApp.Domain.Feedbacks;
public sealed class Feedback:Entity
{
    public string Message { get; set; } = default!;
    public string Subject { get; set; } = default!;
   

}
