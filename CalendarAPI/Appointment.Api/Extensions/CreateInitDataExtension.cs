using Appointment.Api.Services.Auth.Interfaces;
using Appointment.Data;
using Appointment.Domain;
using Appointment.Domain.Entities.Identity;
using Appointment.Domain.Entities;
using Appointment.Domain.Services.Auth;
using Microsoft.AspNetCore.Identity;

namespace Appointment.Api.Extensions
{
    public static class CreateInitDataExtension
    {
        private static IUnitOfWork unitOfWork;
        private static IPasswordService passwordService;

        public const string SuperAdminEmail = "admin@admin.admin";
        public const string SuperAdminPassword = "Admin123";

        public const string AdminEmail = "admin@admin.com";
        public const string AdminPassword = "Admin123";

        public const string UserEmail = "user@user.user";
        public const string UserPassword = "User1234";

        public static async Task<IApplicationBuilder> CreateInitData(this IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            passwordService = serviceProvider.GetRequiredService<IPasswordService>();

            await CreateInitUsersAndAdmins();

            return applicationBuilder;
        }

        private static async Task CreateInitUsersAndAdmins()
        {
            if(unitOfWork.Users.Any(u => u.Email.Equals(SuperAdminEmail)))
            {
                return;
            }
            var superAdminPasswordHash = passwordService.Hash(SuperAdminPassword);
            var superAdmin = new User(SuperAdminEmail, "Admin", "Admin", SuperAdminEmail, superAdminPasswordHash,
                Role.SuperAdmin, null, true);
            superAdmin.Activate();

            await unitOfWork.Users.AddAsync(superAdmin);
            await unitOfWork.SaveChangesAsync();

            if(unitOfWork.Users.Any(u => u.Email.Equals(AdminEmail)))
            {
                return;
            }

            var adminPasswordHash = passwordService.Hash(AdminPassword);
            var admin = new User(AdminEmail, "Admin", "Admin", AdminEmail, adminPasswordHash,
               Role.Admin);
            admin.Activate();
            await unitOfWork.Users.AddAsync(admin);
            await unitOfWork.SaveChangesAsync();

            if(unitOfWork.Users.Any(u => u.Email.Equals(UserEmail)))
            {
                return;
            }

            var userPasswordHash = passwordService.Hash(UserPassword);
            var user = new User(UserEmail, "User", "user", UserEmail, userPasswordHash,
              Role.User);
            user.Activate();
            await unitOfWork.Users.AddAsync(user);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
