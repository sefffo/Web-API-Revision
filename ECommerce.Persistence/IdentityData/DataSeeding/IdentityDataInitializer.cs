using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.Domain.Interfaces;
using ECommerce.Persistence.Data.IdentityData;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Persistence.IdentityData.DataSeeding
{
    public class IdentityDataInitializer(UserManager<AppUser> userManager , RoleManager<IdentityRole> roleManager , ILogger<IdentityDataInitializer> logger) : IDataInitializer
    {
        public async Task Initialize()
        {
            try
            {

                if(!roleManager.Roles.Any())
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));

                    //await roleManager.CreateAsync(new IdentityRole("Customer"));

                }
                if (!userManager.Users.Any())
                {
                    var user = new AppUser
                    {
                        UserName = "SaifLotfy",
                        Email = "saif@gmail.com",
                        DisplayName = "Saif Lotfy",
                        PhoneNumber = "01000000000"

                    };
                    var user2 = new AppUser
                    {
                        UserName = "OmarLotfy",
                        Email = "Omar@gmail.com",
                        DisplayName = "Omar Lotfy",
                        PhoneNumber = "01000000001"

                    };

                    await userManager.CreateAsync(user, "Pa$$w0rd");
                    await userManager.CreateAsync(user2, "Pa$$w0rd");

                    await userManager.AddToRoleAsync(user, "SuperAdmin");
                    await userManager.AddToRoleAsync(user2, "Admin");
                }

            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"An error occurred while seeding identity data. \n Message : {ex.Message}");
            }
        }
    }
}
