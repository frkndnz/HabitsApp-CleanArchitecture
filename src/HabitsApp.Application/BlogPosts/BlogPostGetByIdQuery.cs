using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Blogs;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.BlogPosts;
public sealed record BlogPostGetByIdQuery(Guid Id) : IRequest<Result<BlogPostGetByIdQueryResponse>>;

public sealed record BlogPostGetByIdQueryResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public string CreatorName { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

internal sealed class BlogPostGetByIdQueryHandler
    (IBlogPostRepository blogPostRepository,
    IUrlService urlService,
    UserManager<AppUser> userManager
    ) : IRequestHandler<BlogPostGetByIdQuery, Result<BlogPostGetByIdQueryResponse>>
{
    public async Task<Result<BlogPostGetByIdQueryResponse>> Handle(BlogPostGetByIdQuery request, CancellationToken cancellationToken)
    {
        bool isExist = await blogPostRepository.AnyAsync(b => b.Id == request.Id);

        if (!isExist)
            return Result<BlogPostGetByIdQueryResponse>.Failure("not found blogpost ");

        var blogPost=await blogPostRepository.FirstOrDefaultAsync(b=>b.Id == request.Id);
        var user=await userManager.Users.FirstOrDefaultAsync(u=>u.Id == blogPost!.CreateUserId);
        var response = new BlogPostGetByIdQueryResponse
        {
            Id = blogPost!.Id,
            Content = blogPost.Content,
            ShortDescription = blogPost.ShortDescription,
            Title = blogPost.Title,
            ImageUrl = blogPost.ImageUrl != null ? urlService.GetAbsoluteUrl(blogPost.ImageUrl!) : null,
            CreatedAt = blogPost.CreatedAt,
            CreatorName =user?.FullName ?? string.Empty,
        };

        return Result<BlogPostGetByIdQueryResponse>.Success(response, "success!");
        
    }
}
