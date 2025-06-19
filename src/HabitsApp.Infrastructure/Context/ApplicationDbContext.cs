using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.Blogs;
using HabitsApp.Domain.HabitLogs;
using HabitsApp.Domain.Habits;
using HabitsApp.Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Infrastructure.Context;
public sealed class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions opt) : base(opt)
    {

    }
    public DbSet<Habit> Habits { get; set; }
    public DbSet<HabitLog> HabitLogs { get; set; }
    public DbSet<BlogPost> Blogs { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // builder.Ignore<IdentityUserClaim<Guid>>();  
        //  builder.Ignore<IdentityRoleClaim<Guid>>();
        //  builder.Ignore<IdentityUserToken<Guid>>();
        //  builder.Ignore<IdentityUserLogin<Guid>>();
        //  builder.Ignore<IdentityUserRole<Guid>>();



    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var entires = ChangeTracker.Entries<Entity>();

        HttpContextAccessor httpContextAccessor = new();
        var httpContext = httpContextAccessor.HttpContext;

        Guid? userId = null;

        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var claim = httpContext.User.Claims.FirstOrDefault(u => u.Type == "user_id");
            if (claim != null)
            {
                userId = Guid.Parse(claim.Value);
            }
        }
        if (userId.HasValue)
        {
            foreach (var entry in entires)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Property(p => p.CreatedAt).CurrentValue = DateTime.UtcNow;
                        entry.Property(p => p.CreateUserId).CurrentValue = userId.Value; // Explicitly cast Guid? to Guid  
                        break;
                    case EntityState.Modified:
                        entry.Property(p => p.UpdatedAt).CurrentValue = DateTime.UtcNow;
                        entry.Property(p => p.UpdateUserId).CurrentValue = userId.Value; // Explicitly cast Guid? to Guid  
                        break;
                    case EntityState.Deleted:
                        entry.Property(p => p.DeletedAt).CurrentValue = DateTime.UtcNow;
                        entry.Property(p => p.DeleteUserId).CurrentValue = userId.Value; // Explicitly cast Guid? to Guid  
                        break;
                }
            }
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
