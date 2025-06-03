using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.Habits;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.Application.Habits;
public sealed record HabitUpdateCommand(string Name,string Description,string Color,bool isCompletedToday):IRequest<Result<GetUserHabitsQueryResponse>>
{
   public Guid? Id { get; set; }
}


internal sealed class HabitUpdateCommandHandler(IUnitOfWork unitOfWork,IMapper mapper, IHabitRepository habitRepository) : IRequestHandler<HabitUpdateCommand, Result<GetUserHabitsQueryResponse>>
{
    public async Task<Result<GetUserHabitsQueryResponse>> Handle(HabitUpdateCommand request, CancellationToken cancellationToken)
    {
        var habit = habitRepository.FirstOrDefaultAsync(h => h.Id == request.Id).Result;
        if (habit == null)
        {
            return Result<GetUserHabitsQueryResponse>.Failure("Habit not found.");
        }

        mapper.Map(request, habit);

        habitRepository.Update(habit);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<GetUserHabitsQueryResponse>(habit);
        response.IsCompletedToday = request.isCompletedToday;

        return Result<GetUserHabitsQueryResponse>.Success(response, "Habit successfully updated!");

    }
}
