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
    public List<bool>? WeeklyLogStatus { get; set; }
}

internal sealed class GetUserHabitsQueryHandler : IRequestHandler<GetUserHabitsQuery, Result<List<GetUserHabitsQueryResponse>>>
{
    private readonly IHabitRepository _habitRepository;
    private readonly IHabitLogRepository _habitLogRepository;
    private readonly ICategoryRepository _categoryRepository;
    public GetUserHabitsQueryHandler(IHabitRepository habitRepository, ICategoryRepository categoryRepository, IHabitLogRepository habitLogRepository)
    {
        _habitRepository = habitRepository;
        _habitLogRepository = habitLogRepository;
        _categoryRepository = categoryRepository;

    }

    public async Task<Result<List<GetUserHabitsQueryResponse>>> Handle(GetUserHabitsQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var diff = (int)today.DayOfWeek - (int)DayOfWeek.Monday;
        diff = diff < 0 ? 6 : diff;
        var weekStart = today.AddDays(-diff);
        var weekEnd = weekStart.AddDays(7);


        var response = from habit in _habitRepository.GetAll()
                       where habit.CreateUserId == request.UserId
                       join category in _categoryRepository.GetAll()
                         on habit.CategoryId equals category.Id into categoryGroup
                       from category in categoryGroup.DefaultIfEmpty()
                       select new
                       {
                           Habit = habit,
                           Category = category,
                           Logs = _habitLogRepository.GetAll().
                           Where(log => log.HabitId == habit.Id &&
                           log.CreateUserId == request.UserId &&
                           log.Date >= weekStart &&
                           log.Date <= weekEnd).ToList()
                       };

                       

        var habitWithLogs = await response.ToListAsync();

        var habits = habitWithLogs.Select(item => new GetUserHabitsQueryResponse
        {
            Id = item.Habit.Id,
            Name = item.Habit.Name,
            Color = item.Habit.Color,
            Description = item.Habit.Description,
            IsCompletedToday = item.Logs.Any(log => log.Date.Date == DateTime.UtcNow.Date),
            CategoryId = item.Habit.CategoryId,
            CategoryName = item.Category != null ? item.Category.Name : null,
            WeeklyLogStatus = Enumerable.Range(0, 7)
                                .Select(dayOffSet =>
                                {
                                    var dateToCheck = weekStart.AddDays(dayOffSet).Date;
                                    return item.Logs.Any(log => log.Date.Date == dateToCheck);
                                })
                                .ToList()
        }).ToList();


        if (habits == null || !habits.Any())
        {
            return Result<List<GetUserHabitsQueryResponse>>.Success(null, "No habits found for the user.");
        }
        else
        {
            return Result<List<GetUserHabitsQueryResponse>>.Success(habits, "Habits retrieved successfully.");
        }
    }
}


