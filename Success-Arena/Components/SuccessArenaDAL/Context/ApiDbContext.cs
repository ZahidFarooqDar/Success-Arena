using Microsoft.EntityFrameworkCore;
using SuccessArenaDAL.Base;
using SuccessArenaDomainModels.AppUser;
using SuccessArenaDomainModels.Foundation;
using SuccessArenaDomainModels.v1;

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
        public DbSet<ExamDM> Exams { get; set; }

        public DbSet<PostDM> Posts { get; set; }

        public DbSet<SubjectDM> Subjects { get; set; }

        public DbSet<ExamPostDM> ExamPosts { get; set; }

        public DbSet<PostSubjectDM> PostSubjects { get; set; }

        public DbSet<QuestionDM> Questions { get; set; }

        public DbSet<QuestionOptionDM> QuestionOptions { get; set; }

        public DbSet<TestPackageDM> TestPackages { get; set; }

        public DbSet<UserTestPackageDM> UserTestPackages { get; set; }

        public DbSet<TestAttemptDM> TestAttempts { get; set; }

        public DbSet<TestAttemptAnswerDM> TestAttemptAnswers { get; set; }

        public DbSet<InvoiceDM> Invoices { get; set; }

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
