using AutoMapper;
using SuccessArenaBAL.Foundation.Base;
using SuccessArenaBAL.Foundation.CommonUtils;
using SuccessArenaDAL.Context;
using SuccessArenaDomainModels.AppUser.Login;
using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;
using SuccessArenaServiceModels.Foundation.Base.Interfaces;

namespace SuccessArenaBAL.AppUsers
{
    public abstract class LoginUserProcess<T> : SuccessArenaBalOdataBase<T>
    {
        #region Properties

        protected readonly ILoginUserDetail _loginUserDetail;

        #endregion Properties

        #region Constructor
        public LoginUserProcess(IMapper mapper, ILoginUserDetail loginUserDetail, ApiDbContext apiDbContext)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }
        #endregion Constructor

        #region CRUD 


        #endregion CRUD

        #region Private Functions
        #endregion Private Functions
    }

}
