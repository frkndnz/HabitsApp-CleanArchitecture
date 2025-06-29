using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitsApp.Domain.Abstractions;
public abstract class Entity
{
    public Guid Id { get; set; }
    protected Entity()
    {
        Id = Guid.CreateVersion7();
        CreatedAt = DateTime.UtcNow;
    }


    #region AuditLog
    public DateTime CreatedAt { get; set; }
    public Guid CreateUserId { get; set; } = default!;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdateUserId { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeleteUserId { get; set; }
    #endregion
}
