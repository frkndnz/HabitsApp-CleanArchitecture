using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Users;
using HabitsApp.Domain.Abstractions;
using HabitsApp.Domain.Feedbacks;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.Feedbacks;
public sealed record GetAllFeedbackQuery(
    int Page,
    int PageSize):IRequest<Result<GetAllFeedbackResponse>>;

public class GetAllFeedbackResponse
{
    public List<FeedbackDto> Feedbacks { get; set; } =new(); 
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class FeedbackDto : EntityDto
{
    public Guid Id { get; set; }
    public string Message { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string CreatedUserEmail { get; set; }=default!;
}

internal sealed class GetAllFeedbackQueryHandler(
    UserManager<AppUser> userManager,
    IFeedbackRepository feedbackRepository
    ) : IRequestHandler<GetAllFeedbackQuery, Result<GetAllFeedbackResponse>>
{
    public async Task<Result<GetAllFeedbackResponse>> Handle(GetAllFeedbackQuery request, CancellationToken cancellationToken)
    {
        var query =
            from feedback in feedbackRepository.GetAll()
            join creator in userManager.Users
                on feedback.CreateUserId equals creator.Id into createdByGroup
            from createdBy in createdByGroup.DefaultIfEmpty()
            select new
            {
                Feedback = feedback,
                CreatedBy = createdBy,
            };

        var totalCount = await query.CountAsync(cancellationToken);

        var pagedQuery = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        var pagedUsersData = await pagedQuery.ToListAsync(cancellationToken);
        var userDtos = new List<FeedbackDto>();

        foreach (var item in pagedQuery)
        {
            userDtos.Add(new FeedbackDto()
            {
                Id = item.Feedback.Id,
                Message = item.Feedback.Message,
                Subject = item.Feedback.Subject,
                CreateUserName = item.CreatedBy.FullName,
                CreatedUserEmail=item.CreatedBy.Email!,
                CreatedAt = item.Feedback.CreatedAt,
                CreateUserId = item.CreatedBy.Id,
                DeletedAt = item.Feedback.DeletedAt,
                DeleteUserId = item.Feedback.DeleteUserId,
                UpdatedAt = item.Feedback.UpdatedAt,
                UpdateUserId = item.Feedback.UpdateUserId,
            });
        }
        GetAllFeedbackResponse response = new()
        {
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            Feedbacks = userDtos,
        };

        return Result<GetAllFeedbackResponse>.Success(response, "successfully taked feedbacks with pagination!");
    }
}
