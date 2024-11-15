using Microsoft.AspNetCore.Identity;

namespace Bakr.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [PersonalData]
    public string Name { get; set; } = "";
}
