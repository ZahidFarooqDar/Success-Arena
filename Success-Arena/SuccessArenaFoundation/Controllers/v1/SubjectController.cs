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
    public partial class SubjectController : ApiControllerWithOdataRoot<SubjectSM>
    {
        private readonly SubjectProcess _subjectProcess;

        public SubjectController(SubjectProcess process)
            : base(process)
        {
            _subjectProcess = process;
        }

        #region OData

        [HttpGet]
        [Route("odata")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SubjectSM>>>> GetAsOdata(
            ODataQueryOptions<SubjectSM> oDataOptions)
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
        public async Task<ActionResult<ApiResponse<IEnumerable<SubjectSM>>>> GetAll(
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var listSM = await _subjectProcess.GetAllSubjects(
                skip,
                top);

            return Ok(
                ModelConverter.FormNewSuccessResponse(listSM));
        }

        [HttpGet("active/{skip:int}/{top:int}")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SubjectSM>>>> GetAllActive(
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var listSM = await _subjectProcess.GetAllActiveSubjects(
                skip,
                top);

            return Ok(
                ModelConverter.FormNewSuccessResponse(listSM));
        }

        [HttpGet("available/{skip:int}/{top:int}")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SubjectSM>>>> GetAvailableSubjectsForUsers(
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var listSM = await _subjectProcess.GetAvailableSubjectsForUsers(
                skip,
                top);

            return Ok(
                ModelConverter.FormNewSuccessResponse(listSM));
        }

        [HttpGet("{id:int}")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<SubjectSM>>> GetById(
            int id)
        {
            var sm = await _subjectProcess.GetSubjectById(id);

            return Ok(
                ModelConverter.FormNewSuccessResponse(sm));
        }

        [HttpGet("search")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SubjectSM>>>> Search(
            string searchText,
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var listSM = await _subjectProcess.SearchSubjects(
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
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> SearchSubjectsCount(
            string searchText)
        {
            var response = await _subjectProcess.SearchSubjectsCount(
                searchText);

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        [HttpGet("post/{postId:int}/{skip:int}/{top:int}")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SubjectSM>>>> GetSubjectsByPostForUsers(
            int postId,
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var response = await _subjectProcess.GetSubjectsByPostForUsers(
                postId,
                skip,
                top);

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
            var response = await _subjectProcess.GetAllSubjectsCount();

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        [HttpGet("count/active")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetActiveCount()
        {
            var response = await _subjectProcess.GetAllActiveSubjectsCount();

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        [HttpGet("count/available")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetAvailableCount()
        {
            var response = await _subjectProcess.GetAvailableSubjectsForUsersCount();

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        [HttpGet("post/{postId:int}/count")]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetSubjectsByPostCount(
            int postId)
        {
            var response = await _subjectProcess.GetSubjectsByPostForUsersCount(
                postId);

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        #endregion

        #region Add

        [HttpPost]
        [Authorize(
            AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<SubjectSM>>> Post(
            [FromBody] ApiRequest<SubjectSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;

            if (innerReq == null)
            {
                return BadRequest(
                    ModelConverter.FormNewErrorResponse(
                        "Request data not found.",
                        ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            var addedSM = await _subjectProcess.AddSubject(
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
        public async Task<ActionResult<ApiResponse<SubjectSM>>> Put(
            int id,
            [FromBody] ApiRequest<SubjectSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;

            if (innerReq == null)
            {
                return BadRequest(
                    ModelConverter.FormNewErrorResponse(
                        "Request data not found.",
                        ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            var updatedSM = await _subjectProcess.UpdateSubject(
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
            var response = await _subjectProcess.UpdateStatusOfSubject(
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
            var response = await _subjectProcess.DeleteSubject(id);

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