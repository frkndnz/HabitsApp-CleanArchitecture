using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.Categories;
using HabitsApp.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.Categories;
public sealed record GetCategoriesQuery:IRequest<Result<List<GetCategoriesQueryResponse>>>;

public sealed class GetCategoriesQueryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Emoji { get; set; }
}

internal sealed class GetCategoriesQueryHandler(
    ICategoryRepository categoryRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<GetCategoriesQuery, Result<List<GetCategoriesQueryResponse>>>
{
    public async Task<Result<List<GetCategoriesQueryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        var response = categoryRepository.GetAll()
            .Where(c => c.CreateUserId == userId)
            .Select(c => new GetCategoriesQueryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Emoji = c.Emoji,
            });
        var categories = await response.ToListAsync();

        if (categories is null || !categories.Any())
            return Result<List<GetCategoriesQueryResponse>>.Failure("not found categories");

        return Result<List<GetCategoriesQueryResponse>>.Success(categories, "Categories retrieved successfully");
    }
}
