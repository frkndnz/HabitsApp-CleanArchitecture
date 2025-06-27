using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Feedbacks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitsApp.Infrastructure.Configurations;
public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
      
        builder.Property(f => f.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(f => f.Subject)
            .HasMaxLength(200);
    }
}
