using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.HabitLogs;
using HabitsApp.Domain.Habits;
using HabitsApp.Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Infrastructure.Context;
internal sealed class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>,IUnitOfWork
    {
    public ApplicationDbContext(DbContextOptions opt):base(opt)
    {
        
    }
    public DbSet<Habit> Habits { get; set; }
    public DbSet<HabitLog> HabitLogs { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.Ignore<IdentityUserClaim<Guid>>();  
        builder.Ignore<IdentityRoleClaim<Guid>>();
        builder.Ignore<IdentityUserToken<Guid>>();
        builder.Ignore<IdentityUserLogin<Guid>>();
        builder.Ignore<IdentityUserRole<Guid>>();

       

    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {

        var entires = ChangeTracker.Entries<Entity>();

        HttpContextAccessor httpContextAccessor = new();
        string userIdString = httpContextAccessor.HttpContext!.User.Claims.First(u => u.Type == "user_id").Value;
        Guid userId = Guid.Parse(userIdString);

        foreach (var entry in entires)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(p=>p.CreatedAt).CurrentValue = DateTime.UtcNow;
                    entry.Property(p=>p.CreateUserId).CurrentValue = userId;
                    break;
                case EntityState.Modified:
                    entry.Property(p => p.UpdatedAt).CurrentValue = DateTime.UtcNow;
                    entry.Property(p => p.UpdateUserId).CurrentValue = userId;
                    break;
                case EntityState.Deleted:
                    entry.Property(p => p.DeletedAt).CurrentValue = DateTime.UtcNow;
                    entry.Property(p => p.DeleteUserId).CurrentValue = userId;
                    break;
            }
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
