using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.Categories;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.Application.Categories;
public sealed record CreateCategoryCommand(
    string Name,
    string? Emoji
    ) : IRequest<Result<CreateCategoryCommandResponse>>;

public class CreateCategoryCommandResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Emoji { get; set; }
}

internal sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork
    ) : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryCommandResponse>>
{
    public async Task<Result<CreateCategoryCommandResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        bool exist=await categoryRepository.AnyAsync(c=>c.Name==request.Name);
        if (exist )
            return Result<CreateCategoryCommandResponse>.Failure("zaten var");

        var category = new Category()
        {
            Name = request.Name.Trim(),
            Emoji = request.Emoji,
        };
        categoryRepository.Add(category);

        await unitOfWork.SaveChangesAsync();

        var response = new CreateCategoryCommandResponse()
        {
            Name = category.Name,
            Emoji = category.Emoji,
            Id = category.Id
        };

        return Result<CreateCategoryCommandResponse>.Success(response,"succesfully!");
    }
}
