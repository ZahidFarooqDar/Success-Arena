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
    public class TestPackageProcess
    : SuccessArenaBalOdataBase<TestPackageSM>
    {
        protected readonly ILoginUserDetail _loginUserDetail;

        public TestPackageProcess(
            IMapper mapper,
            ILoginUserDetail loginUserDetail,
            ApiDbContext apiDbContext)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }

        #region OData

        public override async Task<IQueryable<TestPackageSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.TestPackages;

            IQueryable<TestPackageSM> retSM =
                await MapEntityAsToQuerable<TestPackageDM, TestPackageSM>(_mapper, entitySet);

            return retSM;
        }

        #endregion

        #region Validate
        private async Task ValidateTestPackageAsync(
    TestPackageSM sm)
        {
            if (sm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Test Package payload is null.",
                    "Please provide valid package details.");
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

            if (sm.SubjectId.HasValue)
            {
                bool subjectExists = await _apiDbContext.Subjects
                    .AnyAsync(x => x.Id == sm.SubjectId);

                if (!subjectExists)
                {
                    throw new SuccessArenaException(
                        ApiErrorTypeSM.InvalidInputData_NoLog,
                        $"Invalid Subject Id : {sm.SubjectId}",
                        "Selected subject does not exist.");
                }
            }

            if (string.IsNullOrWhiteSpace(sm.Name))
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Package name missing.",
                    "Package name is required.");
            }

            if (sm.TotalQuestions <= 0)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Invalid question count.",
                    "Total questions must be greater than zero.");
            }

            if (sm.DurationInMinutes <= 0)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Invalid duration.",
                    "Duration must be greater than zero.");
            }

            if (sm.AllowedAttempts <= 0)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Invalid attempts.",
                    "Allowed attempts must be greater than zero.");
            }

            if (!sm.IsFree && sm.Price <= 0)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Invalid package price.",
                    "Paid package must have a valid price.");
            }
        }
        #endregion Validate



        #region Get

        #region Get All

        public async Task<List<TestPackageSM>> GetAllTestPackages(
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.TestPackages
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<TestPackageSM>>(dms);
        }

        public async Task<List<TestPackageSM>> GetActiveTestPackages(
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.TestPackages
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<TestPackageSM>>(dms);
        }

        public async Task<List<TestPackageSM>> GetTestPackagesByPost(
    int postId,
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.TestPackages
                .AsNoTracking()
                .Where(x => x.PostId == postId)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<TestPackageSM>>(dms);
        }

        public async Task<List<TestPackageSM>> GetTestPackagesByPostAndSubject(
    int postId,
    int subjectId,
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.TestPackages
                .AsNoTracking()
                .Where(x =>
                    x.PostId == postId &&
                    x.SubjectId == subjectId)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<TestPackageSM>>(dms);
        }
        public async Task<List<TestPackageSM>>
    GetAvailableTestPackagesForUsers(
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.TestPackages
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.Post.IsActive &&
                    (!x.SubjectId.HasValue ||
                     x.Subject.IsActive))
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<TestPackageSM>>(dms);
        
        }

        public async Task<List<TestPackageSM>> GetTestPackagesByPostForUsers(
    int postId,
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.TestPackages
                .AsNoTracking()
                .Where(x =>
                    x.PostId == postId
                    && x.IsActive
                    && x.Post.IsActive
                    && (!x.SubjectId.HasValue ||
                        x.Subject.IsActive))
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<TestPackageSM>>(dms);
        }

        public async Task<List<TestPackageSM>> GetTestPackagesByPostAndSubjectForUsers(
    int postId,
    int subjectId,
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.TestPackages
                .AsNoTracking()
                .Where(x =>
                    x.PostId == postId
                    && x.SubjectId == subjectId
                    && x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<TestPackageSM>>(dms);
        }

        public async Task<List<TestPackageSM>> GetFreeTestPackagesForUsers(
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.TestPackages
                .AsNoTracking()
                .Where(x =>
                    x.IsFree
                    && x.IsActive
                    && x.Post.IsActive
                    && (!x.SubjectId.HasValue ||
                        x.Subject.IsActive))
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<TestPackageSM>>(dms);
        }

        #endregion Get All

        #region Count

        public async Task<IntResponseRoot> GetAllTestPackagesCount()
        {
            var count = await _apiDbContext.TestPackages
                .AsNoTracking()
                .CountAsync();

            return new IntResponseRoot(
                count,
                "Total packages count.");
        }

        public async Task<IntResponseRoot> GetActiveTestPackagesCount()
        {
            var count = await _apiDbContext.TestPackages
                .AsNoTracking()
                .CountAsync(x => x.IsActive);

            return new IntResponseRoot(
                count,
                "Total active packages count.");
        }

        public async Task<IntResponseRoot> GetTestPackageCountByPost(
    int postId)
        {
            var count = await _apiDbContext.TestPackages
                .AsNoTracking()
                .CountAsync(x =>
                    x.PostId == postId);

            return new IntResponseRoot(
                count,
                "Total packages count.");
        }

        public async Task<IntResponseRoot> GetTestPackageCountByPostAndSubject(
    int postId,
    int subjectId)
        {
            var count = await _apiDbContext.TestPackages
                .AsNoTracking()
                .CountAsync(x =>
                    x.PostId == postId &&
                    x.SubjectId == subjectId);

            return new IntResponseRoot(
                count,
                "Total packages count.");
        }

        public async Task<IntResponseRoot> GetAvailableTestPackageCountByPost(
    int postId)
        {
            var count = await _apiDbContext.TestPackages
                .AsNoTracking()
                .CountAsync(x =>
                    x.PostId == postId
                    && x.IsActive
                    && x.Post.IsActive
                    && (!x.SubjectId.HasValue ||
                        x.Subject.IsActive));

            return new IntResponseRoot(
                count,
                "Total available packages count.");
        }
        public async Task<IntResponseRoot> GetAvailableTestPackageCountByPostAndSubject(
    int postId,
    int subjectId)
        {
            var count = await _apiDbContext.TestPackages
                .AsNoTracking()
                .CountAsync(x =>
                    x.PostId == postId
                    && x.SubjectId == subjectId
                    && x.IsActive
                    && x.Post.IsActive
                    && x.Subject.IsActive);

            return new IntResponseRoot(
                count,
                "Total available packages count.");
        }


        #endregion Count

        #region Get By Id
        public async Task<TestPackageSM> GetTestPackageById(
    int id)
        {
            var dm = await _apiDbContext.TestPackages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Test Package not found. Id: {id}",
                    "Package details not found.");
            }

            return _mapper.Map<TestPackageSM>(dm);
        }

        public async Task<TestPackageSM> GetTestPackageByIdForUsers(
    int id)
        {
            var dm = await _apiDbContext.TestPackages
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == id
                    && x.IsActive
                    && x.Post.IsActive
                    && (!x.SubjectId.HasValue ||
                        x.Subject.IsActive));

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Package not found. Id: {id}",
                    "Package details not found.");
            }

            return _mapper.Map<TestPackageSM>(dm);
        }

        #endregion Get By Id




        #endregion Get

        #region Add

        public async Task<TestPackageSM> AddTestPackage(
    TestPackageSM sm)
        {
            await ValidateTestPackageAsync(sm);

            var dm = _mapper.Map<TestPackageDM>(sm);

            dm.CreatedBy = _loginUserDetail.LoginId;
            dm.CreatedOnUTC = DateTime.UtcNow;

            await _apiDbContext.TestPackages.AddAsync(dm);

            await _apiDbContext.SaveChangesAsync();

            return await GetTestPackageById(dm.Id);
        }

        #endregion Add

        #region Update

        public async Task<TestPackageSM> UpdateTestPackage(
    int id,
    TestPackageSM sm)
        {
            await ValidateTestPackageAsync(sm);

            var dm = await _apiDbContext.TestPackages
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Package not found. Id: {id}",
                    "Package details not found.");
            }

            sm.Id = dm.Id;

            _mapper.Map(sm, dm);

            dm.LastModifiedBy = _loginUserDetail.LoginId;
            dm.LastModifiedOnUTC = DateTime.UtcNow;

            await _apiDbContext.SaveChangesAsync();

            return await GetTestPackageById(id);
        }       


        public async Task<BoolResponseRoot>
    UpdateStatusOfTestPackage(
    int id,
    bool status)
        {
            var dm = await _apiDbContext.TestPackages
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Package not found. Id: {id}",
                    "Package details not found.");
            }

            dm.IsActive = status;
            dm.LastModifiedBy = _loginUserDetail.LoginId;
            dm.LastModifiedOnUTC = DateTime.UtcNow;

            await _apiDbContext.SaveChangesAsync();

            return new BoolResponseRoot(
                true,
                "Package status updated successfully.");
        }

        #endregion Update

        #region Delete
        public async Task<DeleteResponseRoot>
    DeleteTestPackage(
    int id)
        {
            var dm = await _apiDbContext.TestPackages
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                return new DeleteResponseRoot(
                    false,
                    "Package not found.");
            }

            bool hasUsers = await _apiDbContext.UserTestPackages
                .AnyAsync(x => x.TestPackageId == id);

            if (hasUsers)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Package {id} is already assigned.",
                    "Package cannot be deleted because users are using it.");
            }

            _apiDbContext.TestPackages.Remove(dm);

            await _apiDbContext.SaveChangesAsync();

            return new DeleteResponseRoot(
                true,
                "Package deleted successfully.");
        }
        #endregion Delete

    }
}