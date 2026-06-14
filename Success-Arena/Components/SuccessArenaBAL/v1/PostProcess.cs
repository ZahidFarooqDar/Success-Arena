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
    public class PostProcess : SuccessArenaBalOdataBase<PostSM>
    {
        protected readonly ILoginUserDetail _loginUserDetail;

        public PostProcess(
            IMapper mapper,
            ILoginUserDetail loginUserDetail,
            ApiDbContext apiDbContext)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }

        #region OData

        public override async Task<IQueryable<PostSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.Posts;

            IQueryable<PostSM> retSM =
                await MapEntityAsToQuerable<PostDM, PostSM>(_mapper, entitySet);

            return retSM;
        }

        #endregion

        #region Get

        public async Task<List<PostSM>> GetAllPosts(int skip, int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Posts
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<PostSM>>(dms);
        }

        public async Task<List<PostSM>> GetAllActivePosts(int skip, int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Posts
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<PostSM>>(dms);
        }

        public async Task<List<PostSM>> GetAvailablePostsForUsers(
            int examId,
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Posts
                .AsNoTracking()
                .Where(p =>
                    p.IsActive &&
                    p.ExamPosts.Any(ep =>
                        ep.ExamId == examId &&
                        ep.Exam.IsActive) &&
                    p.PostSubjects.Any() &&
                    p.Questions.Count(q => q.IsActive) >= 50)
                .OrderBy(x => x.Name)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<PostSM>>(dms);
        }

        public async Task<IntResponseRoot> GetAvailablePostsForUsersCount(
            int examId)
        {

            var count = await _apiDbContext.Posts
                .AsNoTracking()
                .Where(p =>
                    p.IsActive &&
                    p.ExamPosts.Any(ep =>
                        ep.ExamId == examId &&
                        ep.Exam.IsActive) &&
                    p.PostSubjects.Any() &&
                    p.Questions.Count(q => q.IsActive) >= 50)
                .OrderBy(x => x.Name)
                .CountAsync();
            return new IntResponseRoot(count, "Total available posts count fetched successfully.");
        }

        public async Task<PostSM> GetPostById(int id)
        {
            var dm = await _apiDbContext.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Post not found. Id: {id}",
                    "Post details not found.");
            }

            return _mapper.Map<PostSM>(dm);
        }

        public async Task<List<PostSM>> SearchPosts(
            string searchText,
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            searchText ??= string.Empty;

            var dms = await _apiDbContext.Posts
                .AsNoTracking()
                .Where(x =>
                    x.Name.Contains(searchText) ||
                    x.Code.Contains(searchText))
                .OrderBy(x => x.Name)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<PostSM>>(dms);
        }

        public async Task<IntResponseRoot> SearchPostsCount(
            string searchText)
        {
            searchText ??= string.Empty;

            var count = await _apiDbContext.Posts
                .AsNoTracking()
                .Where(x =>
                    x.Name.Contains(searchText) ||
                    x.Code.Contains(searchText))
                .CountAsync();

            return new IntResponseRoot(
                count,
                "Total search count.");
        }

        #endregion

        #region Count

        public async Task<IntResponseRoot> GetAllPostsCount()
        {
            int count = await _apiDbContext.Posts
                .AsNoTracking()
                .CountAsync();

            return new IntResponseRoot(
                count,
                "Total posts count fetched successfully.");
        }

        public async Task<IntResponseRoot> GetAllActivePostsCount()
        {
            int count = await _apiDbContext.Posts
                .AsNoTracking()
                .CountAsync(x => x.IsActive);

            return new IntResponseRoot(
                count,
                "Total active posts count fetched successfully.");
        }

        public async Task<IntResponseRoot> GetAvailablePostsCountForUsers(
            int examId)
        {
            int count = await _apiDbContext.Posts
                .AsNoTracking()
                .Where(p =>
                    p.IsActive &&
                    p.ExamPosts.Any(ep =>
                        ep.ExamId == examId &&
                        ep.Exam.IsActive) &&
                    p.PostSubjects.Any() &&
                    p.Questions.Count(q => q.IsActive) >= 50)
                .CountAsync();

            return new IntResponseRoot(
                count,
                "Total available posts count fetched successfully.");
        }

        #endregion

        #region Exists

        public async Task<bool> PostExists(int id)
        {
            return await _apiDbContext.Posts
                .AnyAsync(x => x.Id == id);
        }

        public async Task<bool> PostNameExists(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            return await _apiDbContext.Posts
                .AnyAsync(x => x.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> PostCodeExists(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            return await _apiDbContext.Posts
                .AnyAsync(x => x.Code.ToLower() == code.ToLower());
        }

        #endregion

        #region Validation

        private async Task ValidatePostAsync(PostSM sm, int? id = null)
        {
            if (sm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Post payload is null.",
                    "Please provide valid post details.");
            }

            if (string.IsNullOrWhiteSpace(sm.Name))
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Post name is empty.",
                    "Post name is required.");
            }

            if (string.IsNullOrWhiteSpace(sm.Code))
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Post code is empty.",
                    "Post code is required.");
            }

            sm.Name = sm.Name.Trim();
            sm.Code = sm.Code.Trim();

            bool duplicateName = await _apiDbContext.Posts
                .AnyAsync(x =>
                    x.Id != (id ?? 0) &&
                    x.Name.ToLower() == sm.Name.ToLower());

            if (duplicateName)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Duplicate Post Name detected. Name: {sm.Name}",
                    "A post with the same name already exists.");
            }

            bool duplicateCode = await _apiDbContext.Posts
                .AnyAsync(x =>
                    x.Id != (id ?? 0) &&
                    x.Code.ToLower() == sm.Code.ToLower());

            if (duplicateCode)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Duplicate Post Code detected. Code: {sm.Code}",
                    "A post with the same code already exists.");
            }
        }

        #endregion

        #region Add

        public async Task<PostSM> AddPost(PostSM sm)
        {
            await ValidatePostAsync(sm);

            var dm = _mapper.Map<PostDM>(sm);

            dm.CreatedBy = _loginUserDetail.LoginId;
            dm.CreatedOnUTC = DateTime.UtcNow;

            await _apiDbContext.Posts.AddAsync(dm);

            int affectedRows = await _apiDbContext.SaveChangesAsync();

            if (affectedRows > 0)
            {
                return await GetPostById(dm.Id);
            }

            throw new SuccessArenaException(
                ApiErrorTypeSM.Fatal_Log,
                "Failed to insert Post record.",
                "Unable to create post. Please try again later.");
        }

        #endregion

        #region Update

        public async Task<PostSM> UpdatePost(int id, PostSM sm)
        {
            await ValidatePostAsync(sm, id);

            var dm = await _apiDbContext.Posts
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Post not found. Id: {id}",
                    "Post details not found.");
            }

            sm.Id = dm.Id;

            _mapper.Map(sm, dm);

            dm.LastModifiedBy = _loginUserDetail.LoginId;
            dm.LastModifiedOnUTC = DateTime.UtcNow;

            int affectedRows = await _apiDbContext.SaveChangesAsync();

            if (affectedRows > 0)
            {
                return await GetPostById(id);
            }

            throw new SuccessArenaException(
                ApiErrorTypeSM.Fatal_Log,
                $"Failed to update Post. Id: {id}",
                "Unable to update post. Please try again later.");
        }

        public async Task<BoolResponseRoot> UpdateStatusOfPost(
            int id,
            bool status)
        {
            var dm = await _apiDbContext.Posts
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Post not found. Id: {id}",
                    "Post details not found.");
            }

            if (dm.IsActive == status)
            {
                return new BoolResponseRoot(
                    false,
                    $"Post is already {(status ? "active" : "inactive")}.");
            }

            dm.IsActive = status;
            dm.LastModifiedBy = _loginUserDetail.LoginId;
            dm.LastModifiedOnUTC = DateTime.UtcNow;

            int affectedRows = await _apiDbContext.SaveChangesAsync();

            if (affectedRows > 0)
            {
                return new BoolResponseRoot(
                    true,
                    "Post status updated successfully.");
            }

            throw new SuccessArenaException(
                ApiErrorTypeSM.Fatal_Log,
                $"Failed to update status. Post Id: {id}",
                "Unable to update post status. Please try again later.");
        }

        #endregion

        #region Delete

        public async Task<DeleteResponseRoot> DeletePost(int id)
        {
            var dm = await _apiDbContext.Posts
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                return new DeleteResponseRoot(
                    false,
                    "Post not found.");
            }

            bool isUsed =
                await _apiDbContext.ExamPosts.AnyAsync(x => x.PostId == id) ||
                await _apiDbContext.PostSubjects.AnyAsync(x => x.PostId == id) ||
                await _apiDbContext.Questions.AnyAsync(x => x.PostId == id) ||
                await _apiDbContext.TestPackages.AnyAsync(x => x.PostId == id);

            if (isUsed)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Post Id {id} is referenced by other records.",
                    "This post cannot be deleted because it is currently in use.");
            }

            _apiDbContext.Posts.Remove(dm);

            int affectedRows = await _apiDbContext.SaveChangesAsync();

            if (affectedRows > 0)
            {
                return new DeleteResponseRoot(
                    true,
                    "Post deleted successfully.");
            }

            return new DeleteResponseRoot(
                false,
                "Unable to delete post.");
        }

        #endregion
    }
}