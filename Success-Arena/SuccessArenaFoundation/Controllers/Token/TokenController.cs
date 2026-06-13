using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuccessArenaBAL.Token;
using SuccessArenaConfig.Configuration;
using SuccessArenaFoundation.Controllers.Base;
using SuccessArenaFoundation.Security;
using SuccessArenaServiceModels.AppUser.Login;
using SuccessArenaServiceModels.Enums;
using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;
using SuccessArenaServiceModels.Foundation.Base.Enums;
using SuccessArenaServiceModels.Foundation.Token;
using System.Security.Claims;


namespace SuccessArenaFoundation.Controllers.Token
{
    public partial class TokenController : ApiControllerRoot
    {
        #region Properties

        private readonly TokenProcess _tokenProcess;
        private readonly JwtHandler _jwtHandler;
        private readonly APIConfiguration _apiConfiguration;

        #endregion Properties

        #region Constructor
        public TokenController(TokenProcess TokenProcess, JwtHandler jwtHandler, APIConfiguration aPIConfiguration)
        {
            _tokenProcess = TokenProcess;
            _jwtHandler = jwtHandler;
            _apiConfiguration = aPIConfiguration;
        }
        #endregion Constructor

        #region Validate Login And Generate Token 


        [HttpPost]
        [Route("ValidateLoginAndGenerateToken")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<TokenResponseSM>>> ValidateLoginAndGenerateToken(ApiRequest<TokenRequestSM> apiRequest)
        {
            #region Check Request

            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_Log));
            }

            if (string.IsNullOrWhiteSpace(innerReq.LoginId) || string.IsNullOrWhiteSpace(innerReq.Password) || innerReq.RoleType == RoleTypeSM.Unknown)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessages.Display_InvalidRequiredDataInputs));
            }

            #endregion Check Request

            LoginUserSM userSM = await _tokenProcess.ValidateLoginAndGenerateToken(innerReq);
            if (userSM == null)
            {
                return NotFound(ModelConverter.FormNewErrorResponse("Invalid Credentials",
                    ApiErrorTypeSM.InvalidInputData_Log));
            }
            else if (userSM.LoginStatus == LoginStatusSM.Disabled && userSM.RoleType != RoleTypeSM.CompanyAutomation)
            {
                return Unauthorized(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessages.Display_UserDisabled, ApiErrorTypeSM.Access_Denied_Log));
            }
            else if (userSM.LoginStatus == LoginStatusSM.PasswordResetRequired)
            {
                return Unauthorized(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessages.Display_UserPasswordResetRequired, ApiErrorTypeSM.Access_Denied_Log));
            }
            else if (!userSM.IsEmailConfirmed)
            {
                return Unauthorized(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessages.Display_UserNotVerified, ApiErrorTypeSM.Access_Denied_Log));
            }
            else
            {
                ICollection<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name,innerReq.LoginId),
                    new Claim(ClaimTypes.Role,userSM.RoleType.ToString()),
                    new Claim(ClaimTypes.GivenName,userSM.FirstName + " " + userSM.MiddleName + " " +userSM.LastName ),
                    new Claim(ClaimTypes.Email,userSM.Email),
                    new Claim(DomainConstants.ClaimsRoot.Claim_DbRecordId,userSM.Id.ToString())
                };   
                var expiryDate = DateTime.Now.AddDays(_apiConfiguration.DefaultTokenValidityDays);

                //// creating object of DateTime 
                //DateTime date1 = DateTime.Now;

                //// creating object of DateTime 
                //DateTime date2 = new DateTime(2025, 12,
                //                         31, 11, 59, 59);
                //var x = date2.Subtract(date1);
                //expiryDate = DateTime.Now.AddDays(x.Days);
                var token = await _jwtHandler.ProtectAsync(_apiConfiguration.JwtTokenSigningKey, claims, new DateTimeOffset(DateTime.Now), new DateTimeOffset(expiryDate), "SuccessArena");
                // here if user is derived class, all properties will be sent
                var tokenResponse = new TokenResponseSM()
                {
                    AccessToken = token,
                    LoginUserDetails = userSM,
                    ExpiresUtc = expiryDate
                };
                return Ok(ModelConverter.FormNewSuccessResponse(tokenResponse));
            }
        }

        #endregion Validate Login And Generate Token 

    }
}
