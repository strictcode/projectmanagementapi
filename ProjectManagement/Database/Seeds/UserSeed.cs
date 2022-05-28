using Microsoft.AspNetCore.Identity;
using ProjectManagement.Data.Identity;
using ProjectManagement.Options;

namespace ProjectManagement.Database.Seeds;

public static class UserSeed
{
    public static async Task CreateAdmin(
        IClock clock,
        UserManager<User> userManager,
        DefaultUserSettings defaultUserSettings,
        AppDbContext dbContext
        )
    {
        var admin = new User {
            UserName = defaultUserSettings.UserName,
            Email = defaultUserSettings.UserName,
            EmailConfirmed = true,
        }.SetCreateBy(defaultUserSettings.UserName, clock.GetCurrentInstant());

        var user = await userManager.FindByEmailAsync(admin.Email);
        
        if (user == null)
        {
            var created = await userManager.CreateAsync(admin, defaultUserSettings.Password);

            if (created.Succeeded)
            {
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Something went wrong in creating of user.\n{string.Join("\n", created.Errors.Select(x => x.Description))}");
            }
        }
    }
}
