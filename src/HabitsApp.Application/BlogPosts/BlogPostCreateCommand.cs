using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.Blogs;
using HabitsApp.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HabitsApp.Application.Blogs;
public sealed class BlogPostCreateCommand:IRequest<Result<Guid>>
{
    public string Title { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string Content { get; set; } = default!;
    public IFormFile? Image { get; set; }
}

internal sealed class BlogPostCreateCommandHandler(
    IBlogPostRepository blogRepository,
    IUnitOfWork unitOfWork,
    IFileStorage fileStorage
    ) : IRequestHandler<BlogPostCreateCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(BlogPostCreateCommand request, CancellationToken cancellationToken)
    {
        string? imageUrl = null;
        if(request.Image is { Length: > 0 })
        {
            imageUrl = await fileStorage.UploadFileAsync(request.Image,"blog-images", cancellationToken);
        }
        var blogPost=new BlogPost(request.Title,request.ShortDescription,request.Content,imageUrl);

       blogRepository.Add(blogPost);
       await unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(blogPost.Id, "success");
    }
}
