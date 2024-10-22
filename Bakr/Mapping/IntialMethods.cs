using System.Security.Claims;
using Bakr.Data;
using Bakr.Entities;
using Microsoft.AspNetCore.Identity;

namespace Bakr.Mapping;

abstract public class IntialMethods
{
    public static async Task CreateAdmin(IServiceProvider serviceProvider)
    {
        UserManager<ApplicationUser> userManger = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        string[] roles = ["Admin", "Stuff"];

        string email = "admin@gmail.com";
        string password = "p@ssW0rd";

        ApplicationUser? admin = await userManger.FindByEmailAsync(email);

        if (admin == null)
        {
            Console.WriteLine("I am here. \n\n\n\n");
            admin = new()
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Name = "Yousef Adel Admin"
            };
            await userManger.CreateAsync(admin, password);
            await userManger.AddToRolesAsync(admin, roles);
        } else {
            await userManger.AddToRolesAsync(admin, roles);
        }

        ApplicationUser? stuff = await userManger.FindByEmailAsync("stuff@gmail.com");

        if (stuff == null)
        {
            stuff = new()
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
            anonymous = new()
            {
                UserName = "yousef@gmail.com",
                Email = "yousef@gmail.com",
                EmailConfirmed = true,
            };
            await userManger.CreateAsync(anonymous, password);
        }
    }

    public static async Task CreateRoles(IServiceProvider serviceProvider)
    {
        RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roles = ["Admin", "Stuff"];

        foreach (string role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
    }
}
