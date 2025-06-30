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

namespace HabitsApp.Application.BlogPosts;
public sealed record BlogPostDeleteCommand(
    Guid Id
    ):IRequest<Result<Guid>>;


internal sealed class BlogPostDeleteCommandHandler(
    IBlogPostRepository blogPostRepository,
    IUnitOfWork unitOfWork,
   IFileStorage fileStorage
    
    ) : IRequestHandler<BlogPostDeleteCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(BlogPostDeleteCommand request, CancellationToken cancellationToken)
    {
        bool isExist=await blogPostRepository.AnyAsync(b=>b.Id==request.Id);
        if (!isExist)
            return Result<Guid>.Failure("not found blog post for that id ");

        var blogPost=await blogPostRepository.FirstOrDefaultAsync(b=>b.Id==request.Id);

        if (!string.IsNullOrEmpty(blogPost?.ImageUrl))
        {
            await fileStorage.DeleteFileAsync(blogPost.ImageUrl,"blog-images");
        }

        blogPostRepository.Delete(blogPost!);

        await unitOfWork.SaveChangesAsync(cancellationToken);


        return Result<Guid>.Success(request.Id, "delete operation is success!");

    }
}
