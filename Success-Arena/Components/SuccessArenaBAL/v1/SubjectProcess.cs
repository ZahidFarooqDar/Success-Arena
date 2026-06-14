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
    public class SubjectProcess : SuccessArenaBalOdataBase<SubjectSM>
    {
        protected readonly ILoginUserDetail _loginUserDetail;

        public SubjectProcess(
            IMapper mapper,
            ILoginUserDetail loginUserDetail,
            ApiDbContext apiDbContext)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }

        #region OData

        public override async Task<IQueryable<SubjectSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.Subjects;

            IQueryable<SubjectSM> retSM =
                await MapEntityAsToQuerable<SubjectDM, SubjectSM>(
                    _mapper,
                    entitySet);

            return retSM;
        }

        #endregion

        #region Validation

        private async Task ValidateSubjectAsync(
            SubjectSM sm,
            int? id = null)
        {
            if (sm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Subject payload is null.",
                    "Please provide valid subject details.");
            }

            if (string.IsNullOrWhiteSpace(sm.Name))
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Subject Name is empty.",
                    "Subject name is required.");
            }

            if (string.IsNullOrWhiteSpace(sm.Code))
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Subject Code is empty.",
                    "Subject code is required.");
            }

            sm.Name = sm.Name.Trim();
            sm.Code = sm.Code.Trim();

            bool duplicateName = await _apiDbContext.Subjects
                .AnyAsync(x =>
                    x.Id != (id ?? 0) &&
                    x.Name.ToLower() == sm.Name.ToLower());

            if (duplicateName)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Duplicate Subject Name detected. Name: {sm.Name}",
                    "A subject with the same name already exists.");
            }

            bool duplicateCode = await _apiDbContext.Subjects
                .AnyAsync(x =>
                    x.Id != (id ?? 0) &&
                    x.Code.ToLower() == sm.Code.ToLower());

            if (duplicateCode)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Duplicate Subject Code detected. Code: {sm.Code}",
                    "A subject with the same code already exists.");
            }
        }

        #endregion

        #region Get

        public async Task<List<SubjectSM>> GetAllSubjects(
            int skip,
            int top)
        {
            return _mapper.Map<List<SubjectSM>>(
                await _apiDbContext.Subjects
                    .AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Skip(skip)
                    .Take(top)
                    .ToListAsync());
        }

        public async Task<List<SubjectSM>> GetAllActiveSubjects(
            int skip,
            int top)
        {
            return _mapper.Map<List<SubjectSM>>(
                await _apiDbContext.Subjects
                    .AsNoTracking()
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.Name)
                    .Skip(skip)
                    .Take(top)
                    .ToListAsync());
        }

        public async Task<List<SubjectSM>> GetAvailableSubjectsForUsers(
            int skip,
            int top)
        {
            var subjects = await _apiDbContext.Subjects
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.PostSubjects.Any() &&
                    x.Questions.Count(q => q.IsActive) >= 50)
                .OrderBy(x => x.Name)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<SubjectSM>>(subjects);
        }

        public async Task<SubjectSM> GetSubjectById(int id)
        {
            var dm = await _apiDbContext.Subjects
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Subject not found. Id: {id}",
                    "Subject details not found.");
            }

            return _mapper.Map<SubjectSM>(dm);
        }

        public async Task<List<SubjectSM>> SearchSubjects(
            string searchText,
            int skip,
            int top)
        {
            searchText ??= string.Empty;

            var dms = await _apiDbContext.Subjects
                .AsNoTracking()
                .Where(x =>
                    x.Name.Contains(searchText) ||
                    x.Code.Contains(searchText))
                .OrderBy(x => x.Name)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<SubjectSM>>(dms);
        }

        public async Task<IntResponseRoot> SearchSubjectsCount(
            string searchText)
        {
            searchText ??= string.Empty;

            var count = await _apiDbContext.Subjects
                .AsNoTracking()
                .Where(x =>
                    x.Name.Contains(searchText) ||
                    x.Code.Contains(searchText))
                .CountAsync();

            return new IntResponseRoot(
                count,
                "Total search count.");
        }

        public async Task<List<SubjectSM>> GetSubjectsByPostForUsers(
    int postId,
    int skip,
    int top)
        {
            var dms = await _apiDbContext.Subjects
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.PostSubjects.Any(ps => ps.PostId == postId) &&
                    x.Questions.Count(q => q.IsActive) >= 50)
                .OrderBy(x => x.Name)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<SubjectSM>>(dms);
        }

        #endregion

        #region Count

        public async Task<IntResponseRoot> GetAllSubjectsCount()
        {
            var count = await _apiDbContext.Subjects
                .AsNoTracking()
                .CountAsync();

            return new IntResponseRoot(
                count,
                "Total subjects count fetched successfully.");
        }

        public async Task<IntResponseRoot> GetAllActiveSubjectsCount()
        {
            var count = await _apiDbContext.Subjects
                .AsNoTracking()
                .CountAsync(x => x.IsActive);

            return new IntResponseRoot(
                count,
                "Total active subjects count fetched successfully.");
        }

        public async Task<IntResponseRoot> GetAvailableSubjectsForUsersCount()
        {
            var count = await _apiDbContext.Subjects
                .AsNoTracking()
                .CountAsync(x =>
                    x.IsActive &&
                    x.PostSubjects.Any() &&
                    x.Questions.Count(q => q.IsActive) >= 50);

            return new IntResponseRoot(
                count,
                "Total available subjects count fetched successfully.");
        }

        public async Task<IntResponseRoot> GetSubjectsByPostForUsersCount(
    int postId)
        {
            var count = await _apiDbContext.Subjects
                .AsNoTracking()
                .CountAsync(x =>
                    x.IsActive &&
                    x.PostSubjects.Any(ps => ps.PostId == postId) &&
                    x.Questions.Count(q => q.IsActive) >= 50);

            return new IntResponseRoot(
                count,
                "Total available subjects.");
        }

        #endregion

        #region Exists

        public async Task<bool> SubjectExists(int id)
        {
            return await _apiDbContext.Subjects
                .AnyAsync(x => x.Id == id);
        }

        public async Task<bool> SubjectNameExists(string name)
        {
            return await _apiDbContext.Subjects
                .AnyAsync(x =>
                    x.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> SubjectCodeExists(string code)
        {
            return await _apiDbContext.Subjects
                .AnyAsync(x =>
                    x.Code.ToLower() == code.ToLower());
        }

        #endregion

        #region Add

        public async Task<SubjectSM> AddSubject(
            SubjectSM sm)
        {
            await ValidateSubjectAsync(sm);

            var dm = _mapper.Map<SubjectDM>(sm);

            dm.CreatedBy = _loginUserDetail.LoginId;
            dm.CreatedOnUTC = DateTime.UtcNow;

            await _apiDbContext.Subjects.AddAsync(dm);

            if (await _apiDbContext.SaveChangesAsync() > 0)
            {
                return await GetSubjectById(dm.Id);
            }

            throw new SuccessArenaException(
                ApiErrorTypeSM.Fatal_Log,
                "Failed to insert Subject.",
                "Unable to create subject. Please try again later.");
        }

        #endregion

        #region Update

        public async Task<SubjectSM> UpdateSubject(
            int id,
            SubjectSM sm)
        {
            await ValidateSubjectAsync(sm, id);

            var dm = await _apiDbContext.Subjects
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Subject not found. Id: {id}",
                    "Subject details not found.");
            }

            sm.Id = dm.Id;

            _mapper.Map(sm, dm);

            dm.LastModifiedBy = _loginUserDetail.LoginId;
            dm.LastModifiedOnUTC = DateTime.UtcNow;

            if (await _apiDbContext.SaveChangesAsync() > 0)
            {
                return await GetSubjectById(id);
            }

            throw new SuccessArenaException(
                ApiErrorTypeSM.Fatal_Log,
                $"Failed to update Subject. Id: {id}",
                "Unable to update subject. Please try again later.");
        }

        public async Task<BoolResponseRoot> UpdateStatusOfSubject(
            int id,
            bool status)
        {
            var dm = await _apiDbContext.Subjects
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Subject not found. Id: {id}",
                    "Subject details not found.");
            }

            dm.IsActive = status;
            dm.LastModifiedBy = _loginUserDetail.LoginId;
            dm.LastModifiedOnUTC = DateTime.UtcNow;

            await _apiDbContext.SaveChangesAsync();

            return new BoolResponseRoot(
                true,
                "Subject status updated successfully.");
        }

        #endregion

        #region Delete

        public async Task<DeleteResponseRoot> DeleteSubject(
            int id)
        {
            var dm = await _apiDbContext.Subjects
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                return new DeleteResponseRoot(
                    false,
                    "Subject not found.");
            }

            bool isUsed =
                await _apiDbContext.PostSubjects.AnyAsync(x => x.SubjectId == id)
                || await _apiDbContext.Questions.AnyAsync(x => x.SubjectId == id)
                || await _apiDbContext.TestPackages.AnyAsync(x => x.SubjectId == id);

            if (isUsed)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Subject Id {id} is referenced by other records.",
                    "This subject cannot be deleted because it is currently in use.");
            }

            _apiDbContext.Subjects.Remove(dm);

            if (await _apiDbContext.SaveChangesAsync() > 0)
            {
                return new DeleteResponseRoot(
                    true,
                    "Subject deleted successfully.");
            }

            return new DeleteResponseRoot(
                false,
                "Unable to delete subject.");
        }

        #endregion
    }
}