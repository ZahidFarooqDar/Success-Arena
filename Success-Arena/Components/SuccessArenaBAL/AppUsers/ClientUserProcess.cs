using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SuccessArenaBAL.ExceptionHandler;
using SuccessArenaDAL.Context;
using SuccessArenaDomainModels.AppUser;
using SuccessArenaDomainModels.Enums;
using SuccessArenaServiceModels.AppUser;
using SuccessArenaServiceModels.Enums;
using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;
using SuccessArenaServiceModels.Foundation.Base.Enums;
using SuccessArenaServiceModels.Foundation.Base.Interfaces;

namespace SuccessArenaBAL.AppUsers
{
    public partial class ClientUserProcess : LoginUserProcess<ClientUserSM>
    {
        #region Properties

        private readonly IPasswordEncryptHelper _passwordEncryptHelper;

        #endregion Properties

        #region Constructor

        public ClientUserProcess(
            IMapper mapper,
            ILoginUserDetail loginUserDetail,
            ApiDbContext apiDbContext,
            IPasswordEncryptHelper passwordEncryptHelper)
            : base(mapper, loginUserDetail, apiDbContext)
        {
            _passwordEncryptHelper = passwordEncryptHelper;
        }

        #endregion Constructor

        #region Odata

        /// <summary>
        /// Odata for ClientUserSM
        /// </summary>
        /// <returns>
        /// Returns IQueryable ClientUserSM
        /// </returns>
        public override async Task<IQueryable<ClientUserSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.ClientUsers;

            IQueryable<ClientUserSM> retSM =
                await MapEntityAsToQuerable<ClientUserDM, ClientUserSM>(
                    _mapper,
                    entitySet);

            return retSM;
        }

        #endregion Odata

        #region CRUD

        #region Get All

        /// <summary>
        /// Fetches all client users from database
        /// </summary>
        /// <returns>
        /// List of ClientUserSM
        /// </returns>
        public async Task<List<ClientUserSM>> GetAllClientUsers()
        {
            var dms = await _apiDbContext.ClientUsers
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<ClientUserSM>>(dms);
        }

        #endregion Get All

        #region Get Single

        #region Get By Id

        /// <summary>
        /// Fetches a client user from database using Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ClientUserSM> GetClientUserById(int id)
        {
            ClientUserDM clientUserDM =
                await _apiDbContext.ClientUsers.FindAsync(id);

            if (clientUserDM != null)
            {
                return _mapper.Map<ClientUserSM>(clientUserDM);
            }

            return null;
        }

        #endregion Get By Id

        #region Get By EmailID

        /// <summary>
        /// Gets client user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<ClientUserSM?> GetClientUserByEmail(string email)
        {
            ClientUserDM? clientUserDM =
                await _apiDbContext.ClientUsers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Email.ToLower() == email.ToLower());

            if (clientUserDM != null)
            {
                return _mapper.Map<ClientUserSM>(clientUserDM);
            }

            return null;
        }

        #endregion Get By EmailID

        #endregion Get Single

        #region Add Update

        #region Add User

        #region Add App User

        /// <summary>
        /// Creates new user for application
        /// </summary>
        /// <param name="signUpSM"></param>
        /// <param name="companyCode"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        /// <exception cref="SuccessArenaException"></exception>
        public async Task<BoolResponseRoot?> SignUp(
            ClientUserSM signUpSM)
        {
            if (signUpSM == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Client user signup payload is null.",
                    "Please provide valid details for signup.");
            }

            if (signUpSM.Email.IsNullOrEmpty())
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Email address is missing during signup.",
                    "Please provide an email address.");
            }

            var existingUserWithEmail =
                await GetClientUserByEmail(signUpSM.Email);

            if (existingUserWithEmail != null)
            {
                if (existingUserWithEmail.Email == signUpSM.Email)
                {
                    throw new SuccessArenaException(
                        ApiErrorTypeSM.InvalidInputData_NoLog,
                        $"Duplicate email detected. Email: {signUpSM.Email}",
                        "The email address is already registered.");
                }

                if (existingUserWithEmail.IsEmailConfirmed)
                {
                    return new BoolResponseRoot(
                        true,
                        "Your account already exists. Please login to continue.");
                }
            }

            using (var transaction =
                   await _apiDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    string passwordHash = string.Empty;

                    if (!signUpSM.Password.IsNullOrEmpty())
                    {
                        passwordHash =
                            await _passwordEncryptHelper.ProtectAsync(
                                signUpSM.Password);
                    }

                    var objDM = _mapper.Map<ClientUserDM>(signUpSM);

                    objDM.RoleType = RoleTypeDM.ClientEmployee;
                    objDM.Password = passwordHash;
                    objDM.CreatedBy = _loginUserDetail.LoginId;
                    objDM.CreatedOnUTC = DateTime.UtcNow;
                    objDM.IsEmailConfirmed = true;
                    objDM.IsPhoneNumberConfirmed = false;
                    objDM.LoginStatus = LoginStatusDM.Enabled;

                    await _apiDbContext.ClientUsers.AddAsync(objDM);

                    if (await _apiDbContext.SaveChangesAsync() > 0)
                    {
                        await transaction.CommitAsync();

                        return new BoolResponseRoot(
                            true,
                            "Your account has been created successfully.");
                    }

                    await transaction.RollbackAsync();

                    throw new SuccessArenaException(
                        ApiErrorTypeSM.Fatal_Log,
                        "Failed to save client user details during signup.",
                        "Unable to create account. Please try again later.");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        #endregion Add App User

        #endregion Add User

        #region Update Client

        /// <summary>
        /// Updates Client Details
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objSM"></param>
        /// <returns></returns>
        /// <exception cref="SuccessArenaException"></exception>
        public async Task<ClientUserSM> UpdateUser(
            int userId,
            ClientUserSM objSM)
        {
            if (userId <= 0)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Invalid user id supplied for update.",
                    "Please provide a valid user id.");
            }

            if (objSM == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Client user update payload is null.",
                    "Nothing to update.");
            }

            ClientUserDM objDM =
                await _apiDbContext.ClientUsers.FindAsync(userId);

            if (objDM == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.NoRecord_NoLog,
                    $"Client user not found with Id: {userId}",
                    "User details not found.");
            }

            if (objSM.Email != objDM.Email)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.Access_Denied_Log,
                    "Email update is not allowed.",
                    "The email address cannot be changed.");
            }

            string imageFullPath = null;

            objSM.Id = objDM.Id;
            objSM.Password = objDM.Password;
            objSM.Email = objDM.Email;

            objSM.LoginStatus = (LoginStatusSM)objDM.LoginStatus;
            objSM.IsPhoneNumberConfirmed = objDM.IsPhoneNumberConfirmed;
            objSM.RoleType = (RoleTypeSM)objDM.RoleType;

            var smProperties = objSM.GetType().GetProperties();
            var dmProperties = objDM.GetType().GetProperties();

            foreach (var smProperty in smProperties)
            {
                var smValue = smProperty.GetValue(objSM, null);

                var dmProperty =
                    dmProperties.FirstOrDefault(p =>
                        p.Name == smProperty.Name);

                if (dmProperty != null)
                {
                    var dmValue = dmProperty.GetValue(objDM, null);

                    if ((smValue == null ||
                        (smValue is string strValue &&
                         string.IsNullOrEmpty(strValue)))
                        && dmValue != null)
                    {
                        smProperty.SetValue(objSM, dmValue, null);
                    }
                }
            }

            _mapper.Map(objSM, objDM);

            objDM.LastModifiedBy = _loginUserDetail.LoginId;
            objDM.LastModifiedOnUTC = DateTime.UtcNow;

            if (await _apiDbContext.SaveChangesAsync() > 0)
            {
                return await GetClientUserById(userId);
            }

            throw new SuccessArenaException(
                ApiErrorTypeSM.Fatal_Log,
                $"Failed to update client user. Id: {userId}",
                "Unable to update user details. Please try again later.");
        }

        #endregion Update Client

        #region Check Email/Login Id

        /// <summary>
        /// Check whether email already exists
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<BoolResponseRoot> CheckExistingEmail(string email)
        {
            var existingUser =
                await _apiDbContext.ClientUsers
                    .AsNoTracking()
                    .Where(x =>
                        x.Email.ToLower() == email.ToLower())
                    .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                return new BoolResponseRoot(
                    true,
                    "Email is available.");
            }

            return new BoolResponseRoot(
                false,
                "Email already exists.");
        }

        #endregion Check Email/Login Id

        #endregion Add Update

        #region Delete

        /// <summary>
        /// Deletes Application User from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="SuccessArenaException"></exception>
        public async Task<DeleteResponseRoot> DeleteClientUserById(int id)
        {
            var dm =
                await _apiDbContext.ClientUsers
                    .AsNoTracking()
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

            if (dm == null)
            {
                return new DeleteResponseRoot(
                    false,
                    "User not found.");
            }

            var dmToDelete = new ClientUserDM()
            {
                Id = id
            };

            _apiDbContext.ClientUsers.Remove(dmToDelete);

            if (await _apiDbContext.SaveChangesAsync() > 0)
            {
                return new DeleteResponseRoot(
                    true,
                    "User deleted successfully.");
            }

            throw new SuccessArenaException(
                ApiErrorTypeSM.Fatal_Log,
                $"Failed to delete client user with Id: {id}",
                "Unable to delete user details. Please try again later.");
        }

        #endregion Delete

        #endregion CRUD

        #region Validate User

        private string ValidateUserUsingEmail(
            EmailConfirmationSM objSM)
        {
            if (string.IsNullOrWhiteSpace(objSM.Email))
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Email cannot be empty.",
                    "Please provide a valid email address.");
            }

            var user =
                (from c in _apiDbContext.ClientUsers
                 where c.Email.ToUpper() == objSM.Email.ToUpper()
                 select new
                 {
                     ClientUser = c
                 }).FirstOrDefault();

            if (user == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.NoRecord_NoLog,
                    $"No user found with email '{objSM.Email}'.",
                    "No account exists with the provided email address.");
            }

            return objSM.Email;
        }

        #endregion Validate User

        #region Additional Methods

        #endregion Additional Methods
    }
}