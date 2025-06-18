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
public sealed record GetAllBlogPostsQuery(
    int Page,
    int PageSize,
    string? SearchTerm) : IRequest<Result<GetAllBlogPostsQueryResponse>>;


public sealed record GetAllBlogPostsQueryResponse()
{
    public List<BlogPostDto> BlogPosts { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
public sealed class BlogPostDto()
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;

    public string? ImageUrl { get; set; }
    public string CreatorName { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

internal sealed class GetAllBlogPostsQueryHandler(
    IBlogPostRepository blogPostRepository,
     UserManager<AppUser> userManager,
    IUrlService urlService
    ) : IRequestHandler<GetAllBlogPostsQuery, Result<GetAllBlogPostsQueryResponse>>
{
    public async Task<Result<GetAllBlogPostsQueryResponse>> Handle(GetAllBlogPostsQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await blogPostRepository.CountAsync();

        var pagedBlogPosts = await (from blogPost in blogPostRepository.GetAll()
                                    join creator in userManager.Users
                                    on blogPost.CreateUserId equals creator.Id into createdByGroup
                                    from createdBy in createdByGroup.DefaultIfEmpty()
                                    where string.IsNullOrEmpty(request.SearchTerm)
                        || blogPost.Title!.Contains(request.SearchTerm)
                        || blogPost.ShortDescription!.Contains(request.SearchTerm)
                                    select new BlogPostDto
                                    {
                                        Id = blogPost.Id,
                                        Title = blogPost.Title,
                                        ShortDescription = blogPost.ShortDescription,
                                        ImageUrl = blogPost.ImageUrl != null ? urlService.GetAbsoluteUrl(blogPost.ImageUrl) : null,
                                        CreatorName = createdBy !=null ? createdBy.FullName : "bilinmiyor",
                                        CreatedAt = blogPost.CreatedAt,
                                    })
                 .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();




        if (!pagedBlogPosts.Any() || pagedBlogPosts == null)
            return Result<GetAllBlogPostsQueryResponse>.Success(null, "not found post!");

        GetAllBlogPostsQueryResponse response = new()
        {
            TotalCount = totalCount,
            BlogPosts = pagedBlogPosts,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result<GetAllBlogPostsQueryResponse>.Success(response, "success");
    }
}
