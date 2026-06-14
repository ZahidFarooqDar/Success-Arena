using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using SuccessArenaBAL.v1;
using SuccessArenaFoundation.Controllers.Base;
using SuccessArenaFoundation.Security;
using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;
using SuccessArenaServiceModels.Foundation.Base.Enums;
using SuccessArenaServiceModels.v1;

namespace SuccessArenaFoundation.Controllers.v1
{
    [Route("api/v1/[controller]")]
    public partial class PostController : ApiControllerWithOdataRoot<PostSM>
    {
        private readonly PostProcess _postProcess;

        public PostController(PostProcess process)
            : base(process)
        {
            _postProcess = process;
        }

        #region OData

        [HttpGet]
        [Route("odata")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PostSM>>>> GetAsOdata(
            ODataQueryOptions<PostSM> oDataOptions)
        {
            var retList = await GetAsEntitiesOdata(oDataOptions);

            return Ok(
                ModelConverter.FormNewSuccessResponse(retList));
        }

        #endregion

        #region Get

        [HttpGet("{skip:int}/{top:int}")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PostSM>>>> GetAll(
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var listSM = await _postProcess.GetAllPosts(
                skip,
                top);

            return Ok(
                ModelConverter.FormNewSuccessResponse(listSM));
        }

        [HttpGet("active/{skip:int}/{top:int}")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PostSM>>>> GetAllActive(
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var listSM = await _postProcess.GetAllActivePosts(
                skip,
                top);

            return Ok(
                ModelConverter.FormNewSuccessResponse(listSM));
        }

        [HttpGet("user/{skip:int}/{top:int}")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "ClientEmployee,SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PostSM>>>> GetPostsForUsers(
            int examId,
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var listSM = await _postProcess.GetAvailablePostsForUsers(
                examId,
                skip,
                top);

            return Ok(
                ModelConverter.FormNewSuccessResponse(listSM));
        }

        [HttpGet("{id:int}")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<PostSM>>> GetById(
            int id)
        {
            var sm = await _postProcess.GetPostById(id);

            return Ok(
                ModelConverter.FormNewSuccessResponse(sm));
        }

        [HttpGet("search")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PostSM>>>> Search(
            string searchText,
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var listSM = await _postProcess.SearchPosts(
                searchText,
                skip,
                top);

            return Ok(
                ModelConverter.FormNewSuccessResponse(listSM));
        }

        [HttpGet("search/count")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> SearchPostsCount(
            string searchText)
        {
            var response = await _postProcess.SearchPostsCount(
                searchText);

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        #endregion

        #region Count

        [HttpGet("count")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetCount()
        {
            var response = await _postProcess.GetAllPostsCount();

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        [HttpGet("count/active")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetActiveCount()
        {
            var response = await _postProcess.GetAllActivePostsCount();

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        [HttpGet("count/user")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "ClientEmployee,SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetUserPostCount(int examId)
        {
            var response = await _postProcess.GetAvailablePostsForUsersCount(examId);

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        #endregion

        #region Add

        [HttpPost]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<PostSM>>> Post(
            [FromBody] ApiRequest<PostSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;

            if (innerReq == null)
            {
                return BadRequest(
                    ModelConverter.FormNewErrorResponse(
                        "Request data not found.",
                        ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            var addedSM = await _postProcess.AddPost(
                innerReq);

            return Ok(
                ModelConverter.FormNewSuccessResponse(addedSM));
        }

        #endregion

        #region Update

        [HttpPut("{id:int}")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<PostSM>>> Put(
            int id,
            [FromBody] ApiRequest<PostSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;

            if (innerReq == null)
            {
                return BadRequest(
                    ModelConverter.FormNewErrorResponse(
                        "Request data not found.",
                        ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            var updatedSM = await _postProcess.UpdatePost(
                id,
                innerReq);

            return Ok(
                ModelConverter.FormNewSuccessResponse(updatedSM));
        }

        [HttpPut("{id:int}/status")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> UpdateStatus(
            int id,
            bool status)
        {
            var response = await _postProcess.UpdateStatusOfPost(
                id,
                status);

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        #endregion

        #region Delete

        [HttpDelete("{id:int}")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<DeleteResponseRoot>>> Delete(
            int id)
        {
            var response = await _postProcess.DeletePost(id);

            if (response.DeleteResult)
            {
                return Ok(
                    ModelConverter.FormNewSuccessResponse(response));
            }

            return NotFound(
                ModelConverter.FormNewErrorResponse(
                    response.DeleteMessage,
                    ApiErrorTypeSM.NoRecord_NoLog));
        }

        #endregion
    }
}