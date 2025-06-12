using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Categories;
using HabitsApp.Domain.HabitLogs;
using HabitsApp.Domain.Habits;
using HabitsApp.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.Habits;
public sealed record GetUserHabitsQuery(Guid UserId) : IRequest<Result<List<GetUserHabitsQueryResponse>>>;

public class GetUserHabitsQueryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string Color { get; set; } = default!;
    public bool IsCompletedToday { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
}

internal sealed class GetUserHabitsQueryHandler : IRequestHandler<GetUserHabitsQuery, Result<List<GetUserHabitsQueryResponse>>>
{
    private readonly IHabitRepository _habitRepository;
    private readonly IHabitLogRepository _habitLogRepository;
    private readonly ICategoryRepository _categoryRepository;
    public GetUserHabitsQueryHandler(IHabitRepository habitRepository,ICategoryRepository categoryRepository, IHabitLogRepository habitLogRepository)
    {
        _habitRepository = habitRepository;
        _habitLogRepository = habitLogRepository;
        _categoryRepository = categoryRepository;

    }

    public async Task<Result<List<GetUserHabitsQueryResponse>>> Handle(GetUserHabitsQuery request, CancellationToken cancellationToken)
    {
        var response = from habit in _habitRepository.GetAll()
                       where habit.CreateUserId == request.UserId
                       join category in _categoryRepository.GetAll()
                         on habit.CategoryId equals category.Id into categoryGroup
                       from category in categoryGroup.DefaultIfEmpty()

                      join log in _habitLogRepository.GetAll()
                       .Where(log => log.CreateUserId == request.UserId && log.Date.Date == DateTime.UtcNow.Date)
                        on habit.Id equals log.HabitId into logGroup
                      from log in logGroup.DefaultIfEmpty()
                      select new GetUserHabitsQueryResponse
                        {
                            Id = habit.Id,
                            Name = habit.Name,
                            Color = habit.Color,
                            Description = habit.Description,
                            IsCompletedToday = log != null,
                            CategoryId=habit.CategoryId,
                            CategoryName=category !=null ? category.Name : null,
                            
                        };

        var habits = await response.ToListAsync();

        
        if (habits == null || !habits.Any())
        {
            return Result<List<GetUserHabitsQueryResponse>>.Success(null, "No habits found for the user.");
        }
        else
        {
            return  Result<List<GetUserHabitsQueryResponse>>.Success(habits, "Habits retrieved successfully.");
        }
    }
}


