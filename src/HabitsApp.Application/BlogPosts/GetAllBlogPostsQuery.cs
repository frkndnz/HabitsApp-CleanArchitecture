using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Blogs;
using HabitsApp.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.BlogPosts;
public sealed class GetAllBlogPostsQuery : IRequest<Result<List<GetAllBlogPostsQueryResponse>>>
{
}

public sealed record GetAllBlogPostsQueryResponse()
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;

    public string? ImageUrl { get; set; }
}

internal sealed class GetAllBlogPostsQueryHandler(
    IBlogPostRepository blogPostRepository,
    IUrlService urlService
    ) : IRequestHandler<GetAllBlogPostsQuery, Result<List<GetAllBlogPostsQueryResponse>>>
{
    public async Task<Result<List<GetAllBlogPostsQueryResponse>>> Handle(GetAllBlogPostsQuery request, CancellationToken cancellationToken)
    {
        var blogPosts = await blogPostRepository.GetAll().Select(b => new GetAllBlogPostsQueryResponse()
        {
            Id = b.Id,
            Title = b.Title,
            ShortDescription = b.ShortDescription,
            ImageUrl =b.ImageUrl !=null ? urlService.GetAbsoluteUrl(b.ImageUrl!) : null,
        }).ToListAsync();

        if (!blogPosts.Any() || blogPosts==null)
            return Result<List<GetAllBlogPostsQueryResponse>>.Failure("not found post!");

        return Result<List<GetAllBlogPostsQueryResponse>>.Success(blogPosts, "success");
    }
}
