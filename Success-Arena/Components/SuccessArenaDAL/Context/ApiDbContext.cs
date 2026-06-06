using Microsoft.EntityFrameworkCore;
using SuccessArenaDAL.Base;
using SuccessArenaDomainModels.AppUser;
using SuccessArenaDomainModels.Foundation;

namespace SuccessArenaDAL.Context
{
    public class ApiDbContext : EfCoreContextRoot
    {
        #region Constructor
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        #endregion Constructor

        #region Log Tables
        public DbSet<ErrorLogRoot> ErrorLogRoots { get; set; }

        #endregion Log Tables

        #region App Users
        public DbSet<ApplicationUserDM> ApplicationUsers { get; set; }
        public DbSet<ClientUserDM> ClientUsers { get; set; }

        #endregion App Users

        #region SuccessArena        

        #endregion SuccessArena

        #region On Model Creating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure unique constraint on Email and UserName
            /*modelBuilder.Entity<ClientUserDM>()
                .HasIndex(u => u.EmailId)
                .IsUnique();

            modelBuilder.Entity<ClientUserDM>()
                .HasIndex(u => u.LoginId)
                .IsUnique();*/



            // Seed database with initial data
            DatabaseSeeder<ApiDbContext> seeder = new DatabaseSeeder<ApiDbContext>();
            seeder.SetupDatabaseWithSeedData(modelBuilder);
        }

        #endregion On Model Creating

    }
}
