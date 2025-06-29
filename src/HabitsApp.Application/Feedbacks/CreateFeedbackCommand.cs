using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.Feedbacks;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HabitsApp.Application.Feedbacks;
public sealed record CreateFeedbackCommand
    (
        string Message,
        string Subject
    ):IRequest<Result<string>>;



internal sealed class CreateFeedbackCommandHandler(
    ICurrentUserService currentUserService,
    IFeedbackRepository feedbackRepository,
    IUnitOfWork unitOfWork
    ) : IRequestHandler<CreateFeedbackCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
    {
        Guid userId=currentUserService.UserId;

        Feedback feedback = new()
        {
            Message = request.Message,
            Subject = request.Subject,
        };
         feedbackRepository.Add( feedback );

        await unitOfWork.SaveChangesAsync();

        return Result<string>.Success(feedback.Id.ToString(), "Successfully created feedback.");
    }
}

