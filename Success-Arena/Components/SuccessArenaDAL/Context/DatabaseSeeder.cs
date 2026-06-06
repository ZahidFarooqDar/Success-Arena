using Microsoft.EntityFrameworkCore;
using SuccessArenaDAL.Base;
using SuccessArenaDomainModels.AppUser;
using SuccessArenaDomainModels.Enums;

namespace SuccessArenaDAL.Context
{
    public class DatabaseSeeder<T> where T : EfCoreContextRoot
    {
        public void SetupDatabaseWithSeedData(ModelBuilder modelBuilder)
        {
            var defaultCreatedBy = "SeedAdmin";
            /*SeedDummyCompanyData(modelBuilder, defaultCreatedBy);*/
        }
        //public bool SetupDatabaseWithTestData(T context, Func<string, string> encryptorFunc)
        public async Task<bool> SetupDatabaseWithTestData(T context, Func<string, string> encryptorFunc)
        {
            var defaultCreatedBy = "SeedAdmin";
            var defaultUpdatedBy = "UpdateAdmin";
            var apiDb = context as ApiDbContext;
            if (apiDb != null && apiDb.ApplicationUsers.Count() == 0)
            {
                SeedDummySuperAdminUsers(apiDb, defaultCreatedBy, defaultUpdatedBy, encryptorFunc);
                SeedDummySystemAdminUsers(apiDb, defaultCreatedBy, defaultUpdatedBy, encryptorFunc);
                SeedDummyClientAdminUsers(apiDb, defaultCreatedBy, defaultUpdatedBy, encryptorFunc);

                return true;
            }
            return false;
        }



        #region Data To Entities

        #region Companies
        /*private void SeedDummyCompanyData(ModelBuilder modelBuilder, string defaultCreatedBy)
        {
            var SuccessArenaCompany = new ClientCompanyDetailDM()
            {
                Id = 1,
                Name = "SuccessArena Killer",
                CompanyCode = "123",
                Description = "Software Development Company",
                ContactEmail = "SuccessArena@outlook.com",
                CompanyMobileNumber = "9876542341",
                CompanyWebsite = "www.SuccessArena.com",
                CompanyLogoPath = "wwwroot/content/companies/logos/company.jpg",
                CompanyDateOfEstablishment = new DateTime(1990, 1, 1),
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow
            };
            
            modelBuilder.Entity<ClientCompanyDetailDM>().HasData(codeVisionCompany);
        }*/

        #endregion Companies

        #region Users

        private void SeedDummySuperAdminUsers(ApiDbContext apiDb, string defaultCreatedBy, string defaultUpdatedBy, Func<string, string> encryptorFunc)
        {
            var superUser1 = new ApplicationUserDM()
            {
                RoleType = RoleTypeDM.SuperAdmin,
                FirstName = "Super",
                MiddleName = "Admin",
                EmailId = "saone@email.com",
                LastName = "One",
                LoginId = "super1",
                IsEmailConfirmed = true,
                LoginStatus = LoginStatusDM.Enabled,
                PhoneNumber = "1234567890",
                IsPhoneNumberConfirmed = true,
                PasswordHash = encryptorFunc("super1"),
                ProfilePicturePath = "wwwroot/content/loginusers/profiles/profile.jpg",
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow
            };

            apiDb.ApplicationUsers.Add(superUser1);
            apiDb.SaveChanges();

        }
        private void SeedDummySystemAdminUsers(ApiDbContext apiDb, string defaultCreatedBy, string defaultUpdatedBy, Func<string, string> encryptorFunc)
        {
            var sysUser1 = new ApplicationUserDM()
            {
                RoleType = RoleTypeDM.SystemAdmin,
                FirstName = "System",
                MiddleName = "Admin",
                EmailId = "sysone@email.com",
                LastName = "One",
                LoginId = "system1",
                PhoneNumber = "1234567890",
                ProfilePicturePath = "wwwroot/content/loginusers/profiles/profile.jpg",
                IsEmailConfirmed = true,
                LoginStatus = LoginStatusDM.Enabled,
                IsPhoneNumberConfirmed = true,
                PasswordHash = encryptorFunc("systemadmin1"),
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow
            };

            apiDb.ApplicationUsers.Add(sysUser1);
            apiDb.SaveChanges();

        }
        private void SeedDummyClientAdminUsers(ApiDbContext apiDb, string defaultCreatedBy, string defaultUpdatedBy, Func<string, string> encryptorFunc)
        {
            var cAdmin1 = new ClientUserDM()
            {
                RoleType = RoleTypeDM.ClientAdmin,
                FirstName = "Client",
                MiddleName = "User",
                EmailId = "clientuser1@email.com",
                LastName = "One",
                LoginId = "clientuser1",
                IsEmailConfirmed = true,
                PhoneNumber = "1234567890",
                LoginStatus = LoginStatusDM.Enabled,
                IsPhoneNumberConfirmed = true,
                PasswordHash = encryptorFunc("pass123"),
                ProfilePicturePath = "wwwroot/content/loginusers/profiles/profile.jpg",
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow
            };
            var cAdmin2 = new ClientUserDM()
            {
                RoleType = RoleTypeDM.ClientAdmin,
                FirstName = "Client",
                MiddleName = "user",
                EmailId = "clientuser2@email.com",
                LastName = "Two",
                LoginId = "clientuser2",
                IsEmailConfirmed = true,
                PhoneNumber = "1234567890",
                LoginStatus = LoginStatusDM.Enabled,
                IsPhoneNumberConfirmed = true,
                PasswordHash = encryptorFunc("pass123"),
                ProfilePicturePath = "wwwroot/content/loginusers/profiles/profile.jpg",
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow
            };

            apiDb.ClientUsers.AddRange(cAdmin1, cAdmin2);
            apiDb.SaveChanges();

        }


        #endregion Users

        #region Application Specific Tables

        #endregion Application Specific Tables        

        #endregion Data To Entities

    }
}
