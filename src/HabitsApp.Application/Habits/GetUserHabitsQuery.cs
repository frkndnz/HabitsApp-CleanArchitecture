using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Habits;
using HabitsApp.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HabitsApp.Application.Habits;
public sealed record GetUserHabitsQuery(Guid UserId) : IRequest<Result<List<GetUserHabitsQueryResponse>>>;

public class GetUserHabitsQueryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
}

internal sealed class GetUserHabitsQueryHandler : IRequestHandler<GetUserHabitsQuery, Result<List<GetUserHabitsQueryResponse>>>
{
    private readonly IHabitRepository _habitRepository;

    public GetUserHabitsQueryHandler(IHabitRepository habitRepository)
    {
        _habitRepository = habitRepository;
    }

    public Task<Result<List<GetUserHabitsQueryResponse>>> Handle(GetUserHabitsQuery request, CancellationToken cancellationToken)
    {
        var habits = _habitRepository.GetAll()
            .Where(h => h.CreateUserId == request.UserId)
            .Select(h => new GetUserHabitsQueryResponse
            {
                Id = h.Id,
                Name = h.Name,
                Title = h.Title,
                Description = h.Description
            })
            .ToList();
        if (habits == null || !habits.Any())
        {
            return Task.FromResult(Result<List<GetUserHabitsQueryResponse>>.Success(null, "No habits found for the user."));
        }
        else
        {
            return Task.FromResult( Result<List<GetUserHabitsQueryResponse>>.Success(habits, "Habits retrieved successfully."));
        }
    }
}


