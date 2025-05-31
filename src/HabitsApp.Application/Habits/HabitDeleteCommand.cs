using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.Habits;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.Application.Habits;
public sealed record HabitDeleteCommand(Guid id) : IRequest<Result<string>>;


internal sealed class HabitDeleteCommandHandler(IUnitOfWork unitOfWork, IHabitRepository habitRepository) : IRequestHandler<HabitDeleteCommand, Result<string>>
{
    public async Task<Result<string>> Handle(HabitDeleteCommand request, CancellationToken cancellationToken)
    {
        var habit = await habitRepository.FirstOrDefaultAsync(h => h.Id == request.id);
        if (habit == null)
        {
            return Result<string>.Failure("Habit not found.");
        }
        habitRepository.Delete(habit);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<string>.Success(null, "Habit successfully deleted!");
    }
}


