using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using SuccessArenaBAL.AppUsers;
using SuccessArenaFoundation.Controllers.Base;
using SuccessArenaFoundation.Security;
using SuccessArenaServiceModels.AppUser;
using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;
using SuccessArenaServiceModels.Foundation.Base.Enums;

namespace SuccessArenaFoundation.Controllers.AppUsers
{
    [Route("api/v1/[controller]")]
    public partial class ClientUserController : ApiControllerWithOdataRoot<ClientUserSM>
    {
        private readonly ClientUserProcess _clientUserProcess;

        public ClientUserController(ClientUserProcess process)
            : base(process)
        {
            _clientUserProcess = process;
        }

        #region OData

        [HttpGet]
        [Route("odata")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ClientUserSM>>>> GetAsOdata(
            ODataQueryOptions<ClientUserSM> oDataOptions)
        {
            var retList = await GetAsEntitiesOdata(oDataOptions);

            return Ok(
                ModelConverter.FormNewSuccessResponse(retList));
        }

        #endregion

        #region Get

        [HttpGet]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ClientUserSM>>>> GetAll()
        {
            var listSM = await _clientUserProcess.GetAllClientUsers();

            return Ok(
                ModelConverter.FormNewSuccessResponse(listSM));
        }

        [HttpGet("{id:int}")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<ClientUserSM>>> GetById(
            int id)
        {
            var sm = await _clientUserProcess.GetClientUserById(id);

            return Ok(
                ModelConverter.FormNewSuccessResponse(sm));
        }

        [HttpGet("email")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<ClientUserSM>>> GetByEmail(
            string email)
        {
            var sm = await _clientUserProcess.GetClientUserByEmail(email);

            return Ok(
                ModelConverter.FormNewSuccessResponse(sm));
        }

        [HttpGet("check-email")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> CheckEmail(
            string email)
        {
            var response = await _clientUserProcess.CheckExistingEmail(email);

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        #endregion

        #region Add

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> SignUp(
            [FromBody] ApiRequest<ClientUserSM> apiRequest,
            string companyCode,
            string link)
        {
            var innerReq = apiRequest?.ReqData;

            if (innerReq == null)
            {
                return BadRequest(
                    ModelConverter.FormNewErrorResponse(
                        "Request data not found.",
                        ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            var response = await _clientUserProcess.SignUp(
                innerReq);

            return Ok(
                ModelConverter.FormNewSuccessResponse(response));
        }

        #endregion

        #region Update

        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<ClientUserSM>>> Put(
            int id,
            [FromBody] ApiRequest<ClientUserSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;

            if (innerReq == null)
            {
                return BadRequest(
                    ModelConverter.FormNewErrorResponse(
                        "Request data not found.",
                        ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            var updatedSM = await _clientUserProcess.UpdateUser(
                id,
                innerReq);

            return Ok(
                ModelConverter.FormNewSuccessResponse(updatedSM));
        }

        #endregion

        #region Delete

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = SuccessArenaBearerTokenAuthHandlerRoot.DefaultSchema,
            Roles = "SuperAdmin,SystemAdmin")]
        public async Task<ActionResult<ApiResponse<DeleteResponseRoot>>> Delete(
            int id)
        {
            var response = await _clientUserProcess.DeleteClientUserById(id);

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