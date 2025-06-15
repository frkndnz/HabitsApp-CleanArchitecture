using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions;

namespace HabitsApp.Domain.Blogs;
public sealed class BlogPost:Entity
{
    public string Title { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string Content { get; set; } = default!;

    public string? ImageUrl { get; set; }

    public BlogPost(string title, string shortDescription, string content,string? imageUrl)
    {
        Id = Guid.CreateVersion7();
        Title = title;
        ShortDescription = shortDescription;
        Content = content;
        ImageUrl = imageUrl;
    }
}
