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
    public class ExamProcess : SuccessArenaBalOdataBase<ExamSM>
    {
        protected readonly ILoginUserDetail _loginUserDetail;

        public ExamProcess(
            IMapper mapper,
            ILoginUserDetail loginUserDetail,
            ApiDbContext apiDbContext)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }

        #region OData

        public override async Task<IQueryable<ExamSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.Exams;

            IQueryable<ExamSM> retSM =
                await MapEntityAsToQuerable<ExamDM, ExamSM>(_mapper, entitySet);

            return retSM;
        }

        #endregion

        #region Validation

        private async Task ValidateExamAsync(ExamSM sm, int? id = null)
        {
            if (sm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Exam payload is null.",
                    "Please provide valid exam details.");
            }

            if (string.IsNullOrWhiteSpace(sm.Name))
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Exam Name is empty.",
                    "Exam name is required.");
            }

            if (string.IsNullOrWhiteSpace(sm.Code))
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Exam Code is empty.",
                    "Exam code is required.");
            }

            sm.Name = sm.Name.Trim();
            sm.Code = sm.Code.Trim();

            bool duplicateName = await _apiDbContext.Exams
                .AnyAsync(x =>
                    x.Id != (id ?? 0) &&
                    x.Name.ToLower() == sm.Name.ToLower());

            if (duplicateName)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Duplicate Exam Name detected. Name: {sm.Name}",
                    "An exam with the same name already exists.");
            }

            bool duplicateCode = await _apiDbContext.Exams
                .AnyAsync(x =>
                    x.Id != (id ?? 0) &&
                    x.Code.ToLower() == sm.Code.ToLower());

            if (duplicateCode)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Duplicate Exam Code detected. Code: {sm.Code}",
                    "An exam with the same code already exists.");
            }
        }

        #endregion

        #region Get

        public async Task<List<ExamSM>> GetAllExams(int skip, int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Exams
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<ExamSM>>(dms);
        }

        public async Task<List<ExamSM>> GetAllActiveExams(int skip, int top)
        {
            

            var dms = await _apiDbContext.Exams
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<ExamSM>>(dms);
        }

        public async Task<ExamSM> GetExamById(int id)
        {
            var dm = await _apiDbContext.Exams
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Exam not found. Id: {id}",
                    "Exam details not found.");
            }

            return _mapper.Map<ExamSM>(dm);
        }

        public async Task<List<ExamSM>> SearchExams(
            string searchText,
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            searchText ??= string.Empty;

            var dms = await _apiDbContext.Exams
                .AsNoTracking()
                .Where(x =>
                    x.Name.Contains(searchText) ||
                    x.Code.Contains(searchText))
                .OrderBy(x => x.Name)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<ExamSM>>(dms);
        }

        public async Task<IntResponseRoot> SearchExamsCount(
            string searchText)
        {            
            searchText ??= string.Empty;

            var count =  await _apiDbContext.Exams
                .AsNoTracking()
                .Where(x =>
                    x.Name.Contains(searchText) ||
                    x.Code.Contains(searchText))
                .OrderBy(x => x.Name)
                .CountAsync();

            return new IntResponseRoot(count, "Total Search Count");
        }

        #endregion

        #region Count

        public async Task<IntResponseRoot> GetAllExamsCount()
        {
            int count = await _apiDbContext.Exams
                .AsNoTracking()
                .CountAsync();

            return new IntResponseRoot(
                count,
                "Total exams count fetched successfully.");
        }

        public async Task<IntResponseRoot> GetAllActiveExamsCount()
        {
            int count = await _apiDbContext.Exams
                .AsNoTracking()
                .CountAsync(x => x.IsActive);

            return new IntResponseRoot(
                count,
                "Total active exams count fetched successfully.");
        }

        #endregion

        #region Exists

        public async Task<bool> ExamExists(int id)
        {
            return await _apiDbContext.Exams
                .AnyAsync(x => x.Id == id);
        }

        public async Task<bool> ExamNameExists(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            return await _apiDbContext.Exams
                .AnyAsync(x => x.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> ExamCodeExists(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            return await _apiDbContext.Exams
                .AnyAsync(x => x.Code.ToLower() == code.ToLower());
        }

        #endregion

        #region Add

        public async Task<ExamSM> AddExam(ExamSM sm)
        {
            await ValidateExamAsync(sm);

            var dm = _mapper.Map<ExamDM>(sm);

            dm.CreatedBy = _loginUserDetail.LoginId;
            dm.CreatedOnUTC = DateTime.UtcNow;

            await _apiDbContext.Exams.AddAsync(dm);

            int affectedRows = await _apiDbContext.SaveChangesAsync();

            if (affectedRows > 0)
            {
                return await GetExamById(dm.Id);
            }

            throw new SuccessArenaException(
                ApiErrorTypeSM.Fatal_Log,
                "Failed to insert Exam record.",
                "Unable to create exam. Please try again later.");
        }

        #endregion

        #region Update

        public async Task<ExamSM> UpdateExam(int id, ExamSM sm)
        {
            await ValidateExamAsync(sm, id);

            var dm = await _apiDbContext.Exams
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Exam not found. Id: {id}",
                    "Exam details not found.");
            }

            sm.Id = dm.Id;

            _mapper.Map(sm, dm);

            dm.LastModifiedBy = _loginUserDetail.LoginId;
            dm.LastModifiedOnUTC = DateTime.UtcNow;

            int affectedRows = await _apiDbContext.SaveChangesAsync();

            if (affectedRows > 0)
            {
                return await GetExamById(id);
            }

            throw new SuccessArenaException(
                ApiErrorTypeSM.Fatal_Log,
                $"Failed to update Exam. Id: {id}",
                "Unable to update exam. Please try again later.");
        }

        public async Task<BoolResponseRoot> UpdateStatusOfExam(
            int id,
            bool status)
        {
            var dm = await _apiDbContext.Exams
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Exam not found. Id: {id}",
                    "Exam details not found.");
            }

            if (dm.IsActive == status)
            {
                return new BoolResponseRoot(
                    false,
                    $"Exam is already {(status ? "active" : "inactive")}.");
            }

            dm.IsActive = status;
            dm.LastModifiedBy = _loginUserDetail.LoginId;
            dm.LastModifiedOnUTC = DateTime.UtcNow;

            int affectedRows = await _apiDbContext.SaveChangesAsync();

            if (affectedRows > 0)
            {
                return new BoolResponseRoot(
                    true,
                    "Exam status updated successfully.");
            }

            throw new SuccessArenaException(
                ApiErrorTypeSM.Fatal_Log,
                $"Failed to update status. Exam Id: {id}",
                "Unable to update exam status. Please try again later.");
        }

        #endregion

        #region Delete

        public async Task<DeleteResponseRoot> DeleteExam(int id)
        {
            var dm = await _apiDbContext.Exams
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                return new DeleteResponseRoot(
                    false,
                    "Exam not found.");
            }

            bool isUsed = await _apiDbContext.ExamPosts
                .AnyAsync(x => x.ExamId == id);

            if (isUsed)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Exam Id {id} is referenced by other records.",
                    "This exam cannot be deleted because it is currently in use.");
            }

            _apiDbContext.Exams.Remove(dm);

            int affectedRows = await _apiDbContext.SaveChangesAsync();

            if (affectedRows > 0)
            {
                return new DeleteResponseRoot(
                    true,
                    "Exam deleted successfully.");
            }

            return new DeleteResponseRoot(
                false,
                "Unable to delete exam.");
        }

        #endregion
    }
}