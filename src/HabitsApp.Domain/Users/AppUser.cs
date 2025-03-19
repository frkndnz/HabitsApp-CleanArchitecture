using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HabitsApp.Domain.Users;
public sealed class AppUser:IdentityUser<Guid>
{
    public AppUser()
    {
        Id = Guid.CreateVersion7(); // üretim sıralamasına göre GUID oluşturulur.
    }
    public string FirstName { get; set; }=default!;
    public string LastName { get; set; }=default!;
    public string FullName => $"{FirstName} {LastName}";

    #region AuditLog
    public DateTime CreatedAt { get; set; }
    public Guid CreateUserId { get; set; } = default!;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdateUserId { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeleteUserId { get; set; }
    #endregion
}
