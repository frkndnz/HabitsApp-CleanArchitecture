using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.HabitLogs;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.Application.HabitLogs;
public sealed record HabitLogCreateCommand(
    Guid habitId
    ):IRequest<Result<Guid>>;


internal sealed class HabitLogCreateCommandHandler(ICurrentUserService currentUserService,IUnitOfWork unitOfWork,IHabitLogRepository habitLogRepository) : IRequestHandler<HabitLogCreateCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(HabitLogCreateCommand request, CancellationToken cancellationToken)
    {
        var isExist = await habitLogRepository.AnyAsync(
            h => h.HabitId == request.habitId &&
            h.CreateUserId == currentUserService.UserId &&
            h.Date.Date == DateTime.UtcNow.Date
            );


        if(isExist)
        {
            return Result<Guid>.Failure("Bu alışkanlık zaten bugün tamamlandı");
        }

        var habitLog = new HabitLog()
        {
            HabitId = request.habitId,
        };

        habitLogRepository.Add(habitLog); 

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(habitLog.Id, "başarılı şekilde habit log oluşturuldu.");
    }
}