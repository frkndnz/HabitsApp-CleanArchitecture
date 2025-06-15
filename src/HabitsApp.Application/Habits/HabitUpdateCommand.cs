using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.Categories;
using HabitsApp.Domain.Habits;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.Application.Habits;
public sealed record HabitUpdateCommand(
    string Name,
    string Description,
    string Color,
    Guid? CategoryId,
    bool isCompletedToday):IRequest<Result<GetUserHabitsQueryResponse>>
{
   public Guid? Id { get; set; }
}


internal sealed class HabitUpdateCommandHandler
    (
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IHabitRepository habitRepository,
    ICategoryRepository categoryRepository
    ) : IRequestHandler<HabitUpdateCommand, Result<GetUserHabitsQueryResponse>>
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
        var category = await categoryRepository.FirstOrDefaultAsync(c => c.Id == habit.CategoryId);
        response.IsCompletedToday = request.isCompletedToday;
        response.CategoryName = category!.Name;

        return Result<GetUserHabitsQueryResponse>.Success(response, "Habit successfully updated!");

    }
}
