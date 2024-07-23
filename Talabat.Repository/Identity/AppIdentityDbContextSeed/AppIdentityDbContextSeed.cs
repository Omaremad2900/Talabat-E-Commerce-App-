using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;


namespace Talabat.Repository.Identity.AppIdentityDbContextSeed
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManger) 
        {
            if (!userManger.Users.Any())
            {
                var User = new AppUser()
                {
                    DisplayName = "Omar Emad",
                    Email = "Omaremad2900@gmail.com",
                    UserName = "OmarEmad",
                    PhoneNumber = "01220200255"
                };
                await userManger.CreateAsync(User, "Pa$$w0rd");
            }
        
        
        }
    }
}
