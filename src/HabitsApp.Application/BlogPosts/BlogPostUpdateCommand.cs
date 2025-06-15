using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.Blogs;
using HabitsApp.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HabitsApp.Application.BlogPosts;
public sealed record BlogPostUpdateCommand
    : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string Content { get; set; } = default!;
    public IFormFile? Image { get; set; }

}

internal sealed class BlogPostUpdateCommandHandler
    (
        IBlogPostRepository blogPostRepository,
        IUnitOfWork unitOfWork,
        IFileStorage fileStorage,
        IMapper mapper
    ) : IRequestHandler<BlogPostUpdateCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(BlogPostUpdateCommand request, CancellationToken cancellationToken)
    {
        bool isExist = await blogPostRepository.AnyAsync(b => b.Id == request.Id);
        if (!isExist)
            return Result<Guid>.Failure("blogpost not found!");

        var blogPost = await blogPostRepository.FirstOrDefaultAsync(b => b.Id == request.Id);
        string imageUrl =blogPost!.ImageUrl ?? "";
        if (request.Image != null)
        {
            await fileStorage.DeleteFileAsync(blogPost!.ImageUrl!, "blog-images");
            imageUrl = await fileStorage.UploadFileAsync(request.Image, "blog-images", cancellationToken);
        }

        blogPost = mapper.Map(request, blogPost);
        blogPost.ImageUrl= imageUrl;

        blogPostRepository.Update(blogPost!);

        await unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(blogPost!.Id, "succesfully");

    }
}
