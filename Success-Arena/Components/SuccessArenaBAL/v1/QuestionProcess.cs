using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SuccessArenaBAL.ExceptionHandler;
using SuccessArenaBAL.Foundation.Base;
using SuccessArenaDAL.Context;
using SuccessArenaDomainModels.v1;
using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;
using SuccessArenaServiceModels.Foundation.Base.Enums;
using SuccessArenaServiceModels.Foundation.Base.Interfaces;
using SuccessArenaServiceModels.v1;

namespace SuccessArenaBAL.v1
{
    public class QuestionProcess : SuccessArenaBalOdataBase<QuestionSM>
    {
        protected readonly ILoginUserDetail _loginUserDetail;

        public QuestionProcess(
            IMapper mapper,
            ILoginUserDetail loginUserDetail,
            ApiDbContext apiDbContext)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }

        #region OData

        public override async Task<IQueryable<QuestionSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.Questions;

            IQueryable<QuestionSM> retSM =
                await MapEntityAsToQuerable<QuestionDM, QuestionSM>(_mapper, entitySet);

            return retSM;
        }

        #endregion

        #region Validation

        private async Task ValidateQuestionAsync(
            QuestionSM sm,
            int? id = null)
        {
            if (sm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Question payload is null.",
                    "Please provide valid question details.");
            }

            bool postExists = await _apiDbContext.Posts
                .AnyAsync(x => x.Id == sm.PostId);

            if (!postExists)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Invalid Post Id : {sm.PostId}",
                    "Selected post does not exist.");
            }

            bool subjectExists = await _apiDbContext.Subjects
                .AnyAsync(x => x.Id == sm.SubjectId);

            if (!subjectExists)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Invalid Subject Id : {sm.SubjectId}",
                    "Selected subject does not exist.");
            }

            bool mappingExists = await _apiDbContext.PostSubjects
                .AnyAsync(x =>
                    x.PostId == sm.PostId &&
                    x.SubjectId == sm.SubjectId);

            if (!mappingExists)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Post Subject mapping not found.",
                    "Selected subject is not associated with the selected post.");
            }

            if (string.IsNullOrWhiteSpace(sm.QuestionText) &&
                string.IsNullOrWhiteSpace(sm.QuestionImageUrl))
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Question text and image both empty.",
                    "Question text or image is required.");
            }
        }

        private async Task ValidateQuestionOptions(
    int questionId)
        {
            int optionCount = await _apiDbContext.QuestionOptions
                .CountAsync(x =>
                    x.QuestionId == questionId);

            if (optionCount < 4)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Question {questionId} has less than 4 options.",
                    "Every question must have at least 4 options.");
            }

            int correctCount = await _apiDbContext.QuestionOptions
                .CountAsync(x =>
                    x.QuestionId == questionId &&
                    x.IsCorrect);

            if (correctCount != 1)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Question {questionId} has invalid correct option configuration.",
                    "Question must have exactly one correct option.");
            }
        }

        #endregion

        #region Get

        public async Task<List<QuestionSM>> GetAllQuestions(
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Questions
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<QuestionSM>>(dms);
        }

        public async Task<List<QuestionSM>> GetAllActiveQuestions(
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Questions
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<QuestionSM>>(dms);
        }

        public async Task<QuestionSM> GetQuestionById(
            int id)
        {
            var dm = await _apiDbContext.Questions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Question not found. Id: {id}",
                    "Question details not found.");
            }

            return _mapper.Map<QuestionSM>(dm);
        }

        public async Task<QuestionSM> GetQuestionByIdForUsers(
    int id)
        {
            var dm = await _apiDbContext.Questions
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == id
                    && x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive
                    && x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Question not available. Id: {id}",
                    "Question details not found.");
            }

            return _mapper.Map<QuestionSM>(dm);
        }


        public async Task<List<QuestionSM>> GetRandomQuestionsBySubject(
    int subjectId,
    int count)
        {
            var dms = await _apiDbContext.Questions
                .AsNoTracking()
                .Where(x =>
                    x.SubjectId == subjectId
                    && x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive
                    && x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1)
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<QuestionSM>>(dms);
        }

        public async Task<List<QuestionSM>> GetRandomQuestionsByPostAndSubject(
    int postId,
    int subjectId,
    int count)
        {
            var dms = await _apiDbContext.Questions
                .AsNoTracking()
                .Where(x =>
                    x.PostId == postId
                    && x.SubjectId == subjectId
                    && x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive
                    && x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1)
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<QuestionSM>>(dms);
        }

        public async Task<List<QuestionSM>> GetRandomQuestionsByPost(
    int postId,
    int count)
        {
            var dms = await _apiDbContext.Questions
                .AsNoTracking()
                .Where(x =>
                    x.PostId == postId
                    && x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive
                    && x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1)
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<QuestionSM>>(dms);
        }

        public async Task<List<QuestionSM>> SearchQuestions(
    string searchText,
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            searchText ??= string.Empty;

            var dms = await _apiDbContext.Questions
                .AsNoTracking()
                .Where(x =>
                    (x.QuestionText ?? "").Contains(searchText)
                    || (x.Explanation ?? "").Contains(searchText))
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<QuestionSM>>(dms);
        }


        public async Task<IntResponseRoot> SearchQuestionsCount(
    string searchText)
        {
            searchText ??= string.Empty;

            var count = await _apiDbContext.Questions
                .AsNoTracking()
                .CountAsync(x =>
                    (x.QuestionText ?? "").Contains(searchText)
                    || (x.Explanation ?? "").Contains(searchText));

            return new IntResponseRoot(
                count,
                "Total search count.");
        }
        public async Task<IntResponseRoot> GetActiveQuestionCountBySubject(
    int subjectId)
        {
            var count = await _apiDbContext.Questions
                .AsNoTracking()
                .CountAsync(x =>
                    x.SubjectId == subjectId
                    && x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive
                    && x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1);

            return new IntResponseRoot(
                count,
                "Total active questions count.");
        }

        public async Task<IntResponseRoot> GetActiveQuestionCountByPostAndSubject(
    int postId,
    int subjectId)
        {
            var count = await _apiDbContext.Questions
                .AsNoTracking()
                .CountAsync(x =>
                    x.PostId == postId
                    && x.SubjectId == subjectId
                    && x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive
                    && x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1);

            return new IntResponseRoot(
                count,
                "Total active questions count.");
        }

        public async Task<List<QuestionSM>> GetQuestionsByPost(
            int postId,
            int skip,
            int top)
        {
            var dms = await _apiDbContext.Questions
                .AsNoTracking()
                .Where(x => x.PostId == postId)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<QuestionSM>>(dms);
        }

        public async Task<List<QuestionSM>> GetQuestionsBySubject(
            int subjectId,
            int skip,
            int top)
        {
            var dms = await _apiDbContext.Questions
                .AsNoTracking()
                .Where(x => x.SubjectId == subjectId)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<QuestionSM>>(dms);
        }

        public async Task<List<QuestionSM>> GetQuestionsByPostAndSubject(
            int postId,
            int subjectId,
            int skip,
            int top)
        {
            var dms = await _apiDbContext.Questions
                .AsNoTracking()
                .Where(x =>
                    x.PostId == postId &&
                    x.SubjectId == subjectId)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<QuestionSM>>(dms);
        }

        #endregion

        #region User Methods

        public async Task<List<QuestionSM>> GetAvailableQuestionsForUsers(
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Questions
                .AsNoTracking()
                .Where(x =>
                    x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive
                    && x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<QuestionSM>>(dms);
        }

        public async Task<List<QuestionSM>> GetQuestionsByPostForUsers(
    int postId,
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Questions
                .AsNoTracking()
                .Where(x =>
                    x.PostId == postId
                    && x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive
                    && x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<QuestionSM>>(dms);
        }

        public async Task<List<QuestionSM>> GetQuestionsBySubjectForUsers(
    int subjectId,
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Questions
                .AsNoTracking()
                .Where(x =>
                    x.SubjectId == subjectId
                    && x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive
                    && x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<QuestionSM>>(dms);
        }

        public async Task<List<QuestionSM>> GetQuestionsByPostAndSubjectForUsers(
    int postId,
    int subjectId,
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Questions
                .AsNoTracking()
                .Where(x =>
                    x.PostId == postId
                    && x.SubjectId == subjectId
                    && x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive
                    && x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<QuestionSM>>(dms);
        }

        #endregion

        #region Count

        public async Task<IntResponseRoot> GetAllQuestionsCount()
        {
            var count = await _apiDbContext.Questions
                .AsNoTracking()
                .CountAsync();

            return new IntResponseRoot(
                count,
                "Total questions count fetched successfully.");
        }

        public async Task<IntResponseRoot> GetActiveQuestionCountByPost(
    int postId)
        {
            var count = await _apiDbContext.Questions
                .AsNoTracking()
                .CountAsync(x =>
                    x.PostId == postId
                    && x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive
                    && x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1);

            return new IntResponseRoot(
                count,
                "Total active questions count.");
        }
        public async Task<IntResponseRoot> GetAllActiveQuestionsCount()
        {
            var count = await _apiDbContext.Questions
                .AsNoTracking()
                .CountAsync(x => x.IsActive);

            return new IntResponseRoot(
                count,
                "Total active questions count fetched successfully.");
        }

        public async Task<IntResponseRoot> GetAvailableQuestionsForUsersCount()
        {
            var count = await _apiDbContext.Questions
                .AsNoTracking()
                .CountAsync(x =>
                    x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive
                    && x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1);

            return new IntResponseRoot(
                count,
                "Total available questions count.");
        }

        public async Task<IntResponseRoot> GetQuestionCountByPost(
            int postId)
        {
            var count = await _apiDbContext.Questions
                .AsNoTracking()
                .CountAsync(x => x.PostId == postId);

            return new IntResponseRoot(
                count,
                "Total questions count.");
        }

        public async Task<IntResponseRoot> GetQuestionCountBySubject(
            int subjectId)
        {
            var count = await _apiDbContext.Questions
                .AsNoTracking()
                .CountAsync(x => x.SubjectId == subjectId);

            return new IntResponseRoot(
                count,
                "Total questions count.");
        }

        public async Task<IntResponseRoot> GetQuestionCountByPostAndSubject(
            int postId,
            int subjectId)
        {
            var count = await _apiDbContext.Questions
                .AsNoTracking()
                .CountAsync(x =>
                    x.PostId == postId &&
                    x.SubjectId == subjectId);

            return new IntResponseRoot(
                count,
                "Total questions count.");
        }

        public async Task<IntResponseRoot> GetAvailableQuestionCountByPost(
            int postId)
        {
            var count = await _apiDbContext.Questions
                .AsNoTracking()
                .CountAsync(x =>
                    x.PostId == postId &&
                    x.IsActive &&
                    x.Post.IsActive &&
                    x.Subject.IsActive &&
                    x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1);

            return new IntResponseRoot(
                count,
                "Total available questions count.");
        }

        public async Task<IntResponseRoot> GetAvailableQuestionCountBySubject(
            int subjectId)
        {
            var count = await _apiDbContext.Questions
                .AsNoTracking()
                .CountAsync(x =>
                    x.SubjectId == subjectId &&
                    x.IsActive &&
                    x.Post.IsActive &&
                    x.Subject.IsActive &&
                    x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1);

            return new IntResponseRoot(
                count,
                "Total available questions count.");
        }

        public async Task<IntResponseRoot> GetAvailableQuestionCountByPostAndSubject(
            int postId,
            int subjectId)
        {
            var count = await _apiDbContext.Questions
                .AsNoTracking()
                .CountAsync(x =>
                    x.PostId == postId &&
                    x.SubjectId == subjectId &&
                    x.IsActive &&
                    x.Post.IsActive &&
                    x.Subject.IsActive &&
                    x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1);

            return new IntResponseRoot(
                count,
                "Total available questions count.");
        }

        public async Task<IntResponseRoot> GetRandomQuestionPoolCount(
    int postId,
    int subjectId)
        {
            var count = await _apiDbContext.Questions
                .AsNoTracking()
                .CountAsync(x =>
                    x.PostId == postId
                    && x.SubjectId == subjectId
                    && x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive
                    && x.QuestionOptions.Count() >= 4
                    && x.QuestionOptions.Count(o => o.IsCorrect) == 1);

            return new IntResponseRoot(
                count,
                "Available random question pool count.");
        }


        #endregion

        #region Exists

        public async Task<bool> QuestionExists(
            int id)
        {
            return await _apiDbContext.Questions
                .AnyAsync(x => x.Id == id);
        }

        #endregion

        #region Add

        public async Task<QuestionSM> AddQuestion(
            QuestionSM sm)
        {
            await ValidateQuestionAsync(sm);

            var dm = _mapper.Map<QuestionDM>(sm);

            dm.CreatedBy = _loginUserDetail.LoginId;
            dm.CreatedOnUTC = DateTime.UtcNow;

            await _apiDbContext.Questions.AddAsync(dm);

            var affectedRows = await _apiDbContext.SaveChangesAsync();

            if (affectedRows > 0)
            {
                return await GetQuestionById(dm.Id);
            }

            throw new SuccessArenaException(
                ApiErrorTypeSM.Fatal_Log,
                "Failed to insert Question.",
                "Unable to create question. Please try again later.");
        }

        #endregion

        #region Update

        public async Task<QuestionSM> UpdateQuestion(
            int id,
            QuestionSM sm)
        {
            await ValidateQuestionAsync(sm, id);

            var dm = await _apiDbContext.Questions
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Question not found. Id: {id}",
                    "Question details not found.");
            }

            sm.Id = dm.Id;

            _mapper.Map(sm, dm);

            dm.LastModifiedBy = _loginUserDetail.LoginId;
            dm.LastModifiedOnUTC = DateTime.UtcNow;

            var affectedRows = await _apiDbContext.SaveChangesAsync();

            if (affectedRows > 0)
            {
                return await GetQuestionById(id);
            }

            throw new SuccessArenaException(
                ApiErrorTypeSM.Fatal_Log,
                $"Failed to update Question. Id: {id}",
                "Unable to update question. Please try again later.");
        }

        public async Task<BoolResponseRoot> UpdateStatusOfQuestion(
    int id,
    bool status)
        {
            var dm = await _apiDbContext.Questions
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Question not found. Id: {id}",
                    "Question details not found.");
            }

            if (dm.IsActive == status)
            {
                return new BoolResponseRoot(
                    false,
                    $"Question is already {(status ? "active" : "inactive")}.");
            }

            if (status)
            {
                await ValidateQuestionOptions(id);
            }

            dm.IsActive = status;
            dm.LastModifiedBy = _loginUserDetail.LoginId;
            dm.LastModifiedOnUTC = DateTime.UtcNow;

            await _apiDbContext.SaveChangesAsync();

            return new BoolResponseRoot(
                true,
                "Question status updated successfully.");
        }

        #endregion

        #region Delete

        public async Task<DeleteResponseRoot> DeleteQuestion(
     int id)
        {
            var dm = await _apiDbContext.Questions
                .Include(x => x.QuestionOptions)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                return new DeleteResponseRoot(
                    false,
                    "Question not found.");
            }

            bool isUsed = await _apiDbContext.TestAttemptAnswers
                .AnyAsync(x => x.QuestionId == id);

            if (isUsed)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Question Id {id} is referenced by test attempts.",
                    "This question cannot be deleted because it is currently in use.");
            }

            _apiDbContext.QuestionOptions.RemoveRange(
                dm.QuestionOptions);

            _apiDbContext.Questions.Remove(dm);

            var affectedRows = await _apiDbContext.SaveChangesAsync();

            if (affectedRows > 0)
            {
                return new DeleteResponseRoot(
                    true,
                    "Question deleted successfully.");
            }

            return new DeleteResponseRoot(
                false,
                "Unable to delete question.");
        }

        #endregion

    }
}