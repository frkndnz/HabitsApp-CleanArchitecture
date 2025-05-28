using HabitsApp.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace HabitsApp.WebAPI;

public static class ExtensionsMiddleware
{
    public static async Task CreateFirstUser(WebApplication app)
    {
            Console.WriteLine("test");
        using (var scoped = app.Services.CreateScope())
        {
            var userManager = scoped.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            if (!userManager.Users.Any(p => p.UserName == "admin"))
            {
                AppUser user = new()
                {
                    UserName = "admin",
                    Email = "admin@admin.com",
                    FirstName = "Furkan",
                    LastName="Deniz",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,

                };
               
                var result= await  userManager.CreateAsync(user,"admin2557");
                if(result.Succeeded)
                {
                    user.CreateUserId = user.Id;
                   // userManager.AddToRoleAsync(user, "Admin").Wait();
                    userManager.UpdateAsync(user).Wait();

                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
            }
        }
    }
}
