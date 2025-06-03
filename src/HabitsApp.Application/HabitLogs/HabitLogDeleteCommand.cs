using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.HabitLogs;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.Application.HabitLogs;
public sealed record  HabitLogDeleteCommand(
    Guid habitId,
    DateTime Date
    ):IRequest<Result<string>>;


internal sealed class HabitLogDeleteCommandHandler(IUnitOfWork unitOfWork,IHabitLogRepository habitLogRepository) : IRequestHandler<HabitLogDeleteCommand, Result<string>>
{
    public async Task<Result<string>> Handle(HabitLogDeleteCommand request, CancellationToken cancellationToken)
    {
        var habitLog=await habitLogRepository.FirstOrDefaultAsync(h => h.HabitId == request.habitId && h.Date.Date == request.Date.Date);

        if(habitLog == null)
        {
            return Result<string>.Failure("Habit log not found for the specified date.");
        }

        habitLogRepository.Delete(habitLog);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<string>.Success(null, "Habit log successfully deleted for the specified date.");
    }
}


