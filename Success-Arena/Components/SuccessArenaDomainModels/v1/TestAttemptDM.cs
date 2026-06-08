using SuccessArenaDomainModels.AppUser;
using SuccessArenaDomainModels.Enums;
using SuccessArenaDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuccessArenaDomainModels.v1
{
    public class TestAttemptDM : SuccessArenaDomainModelBase<int>
    {
        [ForeignKey(nameof(ClientUser))]
        public int ClientUserId { get; set; }

        public virtual ClientUserDM ClientUser { get; set; }

        [ForeignKey(nameof(UserTestPackage))]
        public int UserTestPackageId { get; set; }

        public virtual UserTestPackageDM UserTestPackage { get; set; }

        [ForeignKey(nameof(Exam))]
        public int ExamId { get; set; }

        public virtual ExamDM Exam { get; set; }

        [ForeignKey(nameof(Subject))]
        public int? SubjectId { get; set; }

        public virtual SubjectDM? Subject { get; set; }

        public TestAttemptStatusDM Status { get; set; }

        public int TotalQuestions { get; set; }

        public int CorrectAnswers { get; set; }

        public int WrongAnswers { get; set; }

        public decimal Percentage { get; set; }

        public int TimeTakenInMinutes { get; set; }

        public DateTime StartedOn { get; set; }

        public DateTime? SubmittedOn { get; set; }

        public virtual ICollection<TestAttemptAnswerDM> AttemptTestAnswers { get; set; }
            = new List<TestAttemptAnswerDM>();
    }
}