using IdentityModel;
using Mango.Services.Identity.DbContexts;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Mango.Services.Identity.Initializer
{
    public class DbInitializer
    {
        //private readonly ApplicationDbContext _db;
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;

        //public DbInitializer(ApplicationDbContext db,
        //                     UserManager<ApplicationUser> userManager,
        //                     RoleManager<IdentityRole> roleManager)
        //{
        //    _db = db;
        //    _userManager = userManager;
        //    _roleManager = roleManager;
        //}



        public static void EnsureSeedData(WebApplication app)
        {
            using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var _db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                _db.Database.Migrate();

                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();


                if (_roleManager.FindByNameAsync(SD.Admin).Result == null)
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();


                }
                else { return; }

                ApplicationUser adminUser = new ApplicationUser()
                {
                    UserName = "admin1@gmail.com",
                    Email = "admin1@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumber = "1111111111",
                    FirstName = "Magdiel",
                    LastName = "Admin"

                };

                _userManager.CreateAsync(adminUser, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(adminUser, SD.Admin).GetAwaiter().GetResult();


                var templ1 = _userManager.AddClaimsAsync(adminUser, new Claim[]
                {
                new Claim(JwtClaimTypes.Name, $"{adminUser.FirstName} - {adminUser.LastName}"),
                new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
                new Claim(JwtClaimTypes.Role, SD.Admin),
                }).Result;



                ApplicationUser customerUser = new ApplicationUser()
                {
                    UserName = "customer1@gmail.com",
                    Email = "customer1@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumber = "1111111111",
                    FirstName = "Benjamin",
                    LastName = "Cust"

                };

                _userManager.CreateAsync(customerUser, "Cust123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(customerUser, SD.Admin).GetAwaiter().GetResult();


                var templ2 = _userManager.AddClaimsAsync(customerUser, new Claim[]
                {
                new Claim(JwtClaimTypes.Name, $"{customerUser.FirstName} - {customerUser.LastName}"),
                new Claim(JwtClaimTypes.GivenName, customerUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName, customerUser.LastName),
                new Claim(JwtClaimTypes.Role, SD.Customer),
                }).Result;
            }
        }


    }
}
