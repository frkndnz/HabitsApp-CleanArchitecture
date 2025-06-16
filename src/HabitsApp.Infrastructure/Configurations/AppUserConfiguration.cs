using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitsApp.Infrastructure.Configurations;
internal class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasIndex(u=>u.UserName).IsUnique();


        builder.Property(u => u.FirstName).HasColumnType("nvarchar(50)");
        builder.Property(u => u.LastName).HasColumnType("nvarchar(50)");
        builder.Property(u => u.UserName).HasColumnType("nvarchar(50)");
        builder.Property(u => u.Email).HasColumnType("nvarchar(MAX)");

      
    }
}
