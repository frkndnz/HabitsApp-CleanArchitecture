using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.Habits;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HabitsApp.Application.Habits;
public sealed record HabitCreateCommand(
    string Name,
    string Title,
    string? Description
) : IRequest<Result<string>>;

public sealed class HabitCreateCommandValidator : AbstractValidator<HabitCreateCommand>
{
    public HabitCreateCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Habit name is required.");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Habit title is required.");
        RuleFor(x => x.Description).MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}

internal sealed class HabitCreateCommandHandler(IUnitOfWork unitOfWork,IMapper mapper,IHabitRepository habitRepository) : IRequestHandler<HabitCreateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(HabitCreateCommand request, CancellationToken cancellationToken)
    {
        
        Habit habit=mapper.Map<Habit>(request);
        habitRepository.Add(habit);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(null,"habit başarılı bir şekilde oluşturuldu!"); // TODO: Implement et
    }
}
