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
    public partial class ExamController : ApiControllerWithOdataRoot<ExamSM>
    {
        private readonly ExamProcess _examProcess;

        public ExamController(ExamProcess process)
            : base(process)
        {
            _examProcess = process;
        }

        #region OData

        [HttpGet]
        [Route("odata")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ExamSM>>>> GetAsOdata(
            ODataQueryOptions<ExamSM> oDataOptions)
        {
            var retList = await GetAsEntitiesOdata(oDataOptions);

            return Ok(
                ModelConverter.FormNewSuccessResponse(retList));
        }

        #endregion

        #region Get

        [HttpGet("{skip:int}/{top:int}")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ExamSM>>>> GetAll(
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;
            var listSM = await _examProcess.GetAllExams(skip, top);

            return Ok(
                ModelConverter.FormNewSuccessResponse(listSM));
        }

        [HttpGet("active/{skip:int}/{top:int}")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ExamSM>>>> GetAllActive(
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;
            var listSM = await _examProcess.GetAllActiveExams(skip, top);

            return Ok(
                ModelConverter.FormNewSuccessResponse(listSM));
        }

        [HttpGet("{id:int}")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<ExamSM>>> GetById(int id)
        {
            var sm = await _examProcess.GetExamById(id);

            return Ok(
                ModelConverter.FormNewSuccessResponse(sm));
        }

        [HttpGet("search")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ExamSM>>>> Search(
            string searchText,
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var listSM = await _examProcess.SearchExams(
                searchText,
                skip,
                top);

            return Ok(
                ModelConverter.FormNewSuccessResponse(listSM));
        }

        [HttpGet("search/count")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> SearchExamsCount(string searchText)
        {
            var response = await _examProcess.SearchExamsCount(searchText);

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        #endregion

        #region Count

        [HttpGet("count")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetCount()
        {
            var response = await _examProcess.GetAllExamsCount();

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        [HttpGet("count/active")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetActiveCount()
        {
            var response = await _examProcess.GetAllActiveExamsCount();

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        #endregion

        /*#region Exists

        [HttpGet("exists/{id:int}")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> Exists(int id)
        {
            var response = await _examProcess.ExamExists(id);

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        [HttpGet("exists/name")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> ExistsByName(
            string name)
        {
            var response = await _examProcess.ExamNameExists(name);

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        [HttpGet("exists/code")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> ExistsByCode(
            string code)
        {
            var response = await _examProcess.ExamCodeExists(code);

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        #endregion*/

        #region Add

        [HttpPost]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<ExamSM>>> Post(
            [FromBody] ApiRequest<ExamSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;

            if (innerReq == null)
            {
                return BadRequest(
                    ModelConverter.FormNewErrorResponse(
                        "Request data not found.",
                        ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            var addedSM = await _examProcess.AddExam(innerReq);

            return Ok(
                ModelConverter.FormNewSuccessResponse(addedSM));
        }

        #endregion

        #region Update

        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<ExamSM>>> Put(
            int id,
            [FromBody] ApiRequest<ExamSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;

            if (innerReq == null)
            {
                return BadRequest(
                    ModelConverter.FormNewErrorResponse(
                        "Request data not found.",
                        ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            var updatedSM = await _examProcess.UpdateExam(
                id,
                innerReq);

            return Ok(
                ModelConverter.FormNewSuccessResponse(updatedSM));
        }

        [HttpPut("{id:int}/status")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> UpdateStatus(
            int id,
            bool status)
        {
            var response = await _examProcess.UpdateStatusOfExam(
                id,
                status);

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        #endregion

        #region Delete

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<DeleteResponseRoot>>> Delete(
            int id)
        {
            var response = await _examProcess.DeleteExam(id);

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