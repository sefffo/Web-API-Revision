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
    public class IdentityDataInitializer(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<IdentityDataInitializer> logger) : IDataInitializer
    {
        public async Task Initialize()
        {
            try
            {
                // Check each role individually using RoleManager so NormalizedName
                // is always set correctly by the normalizer.
                // This self-heals even if the Roles table already has rows from a
                // previous run that bypassed RoleManager (e.g. missing NormalizedName).
                string[] roles = ["Admin", "SuperAdmin", "User"];

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
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
                    var user3 = new AppUser
                    {
                        UserName = "saif.Dev",
                        Email = "superadmin@ecommerce.com",
                        DisplayName = "Saif Lotfy",
                        PhoneNumber = "01000000001"
                    };

                    await userManager.CreateAsync(user, "Pa$$w0rd");
                    await userManager.CreateAsync(user2, "Pa$$w0rd");

                    await userManager.AddToRoleAsync(user, "SuperAdmin");
                    await userManager.AddToRoleAsync(user2, "Admin");

                    await userManager.CreateAsync(user3, "SuperAdmin@123");
                    await userManager.AddToRoleAsync(user3, "SuperAdmin");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while seeding identity data. \n Message : {ex.Message}");
            }
        }
    }
}
