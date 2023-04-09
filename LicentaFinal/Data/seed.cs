using LicWeb.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace LicWeb.Data
{
    public class seed
    {
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.Profesor))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Profesor));
                if (!await roleManager.RoleExistsAsync(UserRoles.Doctor))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Doctor));
                if (!await roleManager.RoleExistsAsync(UserRoles.Student))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Student));
                //Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                string adminUserEmail = "admin@gmail.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new User()
                    {
                        UserName = "Admin",
                        Email = adminUserEmail,
                        EmailConfirmed = true,
                    };
                    await userManager.CreateAsync(newAdminUser, "123");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                string appProfesorEmail = "profesor@gmail.com";

                var appProfesor = await userManager.FindByEmailAsync(appProfesorEmail);
                if (appProfesor == null)
                {
                    var newProfesorUser = new User()
                    {
                        UserName = "Profesor",
                        Email = appProfesorEmail,
                        EmailConfirmed = true,

                    };
                    await userManager.CreateAsync(newProfesorUser, "123");
                    await userManager.AddToRoleAsync(newProfesorUser, UserRoles.Profesor);
                }
                string appStudentEmail = "student@gmail.com";

                var appStudent = await userManager.FindByEmailAsync(appStudentEmail);
                if (appStudent == null)
                {
                    var newStudentUser = new User()
                    {
                        UserName = "Student",
                        Email = appStudentEmail,
                        EmailConfirmed = true,

                    };
                    await userManager.CreateAsync(newStudentUser, "123");
                    await userManager.AddToRoleAsync(newStudentUser, UserRoles.Student);
                }
                string appDoctorEmail = "doctor@gmail.com";

                var appDoctor = await userManager.FindByEmailAsync(appDoctorEmail);
                if (appDoctor == null)
                {
                    var newDoctorUser = new User()
                    {
                        UserName = "Doctor",
                        Email = appDoctorEmail,
                        EmailConfirmed = true,

                    };
                    await userManager.CreateAsync(newDoctorUser, "123");
                    await userManager.AddToRoleAsync(newDoctorUser, UserRoles.Student);
                }
            }
        }
    }
}
