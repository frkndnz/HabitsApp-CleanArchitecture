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
            var roleManager=scoped.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var configuration=scoped.ServiceProvider.GetRequiredService<IConfiguration>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("User"));
            }

            if (!userManager.Users.Any(p => p.UserName == "admin"))
            {
                AppUser user = new()
                {
                    UserName = "admin",
                    Email = configuration["AdminUser:Email"],
                    FirstName = "Furkan",
                    LastName="Deniz",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    

                };
               
                var result= await userManager.CreateAsync(user, configuration["AdminUser:Password"]!);
                if(result.Succeeded)
                {
                    user.CreateUserId = user.Id;
                    userManager.AddToRoleAsync(user, "Admin").Wait();
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
