using Bakr.Data;
using Bakr.Shared.Entities;
using Microsoft.AspNetCore.Identity;

namespace Bakr.Mapping;

public class InitialMethods(IServiceProvider serviceProvider)
{

    public async Task InitialMethodsCreate() {
        await CreateRoles(serviceProvider.GetRequiredService<RoleManager<IdentityRole>>());
        await CreateAdmin(serviceProvider.GetRequiredService<UserManager<ApplicationUser>>());
        await CreateGenre(serviceProvider.GetRequiredService<ApplicationDbContext>());
    }
    private static async Task CreateAdmin(UserManager<ApplicationUser> userManger)
    {
        string[] roles = ["Admin", "Stuff"];

        string email = "admin@gmail.com";
        string password = "p@ssW0rd";

        ApplicationUser? admin = await userManger.FindByEmailAsync(email);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Name = "Yousef Adel Admin"
            };
            await userManger.CreateAsync(admin, password);
            await userManger.AddToRolesAsync(admin, roles);
        }

        ApplicationUser? stuff = await userManger.FindByEmailAsync("stuff@gmail.com");

        if (stuff == null)
        {
            stuff = new ApplicationUser
            {
                UserName = "stuff@gmail.com",
                Email = "stuff@gmail.com",
                EmailConfirmed = true,
                Name = "Yousef Adel Stuff"
            };
            await userManger.CreateAsync(stuff, password);
            await userManger.AddToRoleAsync(stuff, "Stuff");
        }

        ApplicationUser? anonymous = await userManger.FindByEmailAsync("yousef@gmail.com");

        if (anonymous == null)
        {
            anonymous = new ApplicationUser
            {
                UserName = "yousef@gmail.com",
                Email = "yousef@gmail.com",
                EmailConfirmed = true,
            };
            await userManger.CreateAsync(anonymous, password);
        }
    }

    private static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = ["Admin", "Stuff"];

        foreach (string role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
    }

    private static async Task CreateGenre(ApplicationDbContext dbContext) {
        Genre genre = new()
        {
            Name = "حديد",
        };
        if(dbContext.Genres.FirstOrDefault(g => g.Name == genre.Name) is null) {
            dbContext.Genres.Add(genre);
            await dbContext.SaveChangesAsync();
        }
        genre = new()
        {
            Name = "Cross Fit",
        };
        if(dbContext.Genres.FirstOrDefault(g => g.Name == genre.Name) is null) {
            dbContext.Genres.Add(genre);
            await dbContext.SaveChangesAsync();
        }
    }
}