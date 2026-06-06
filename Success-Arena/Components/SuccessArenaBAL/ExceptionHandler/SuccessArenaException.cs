using SuccessArenaServiceModels.Foundation.Base.Enums;
namespace SuccessArenaBAL.ExceptionHandler
{
    public class SuccessArenaException : ApiExceptionRoot
    {

        public SuccessArenaException(ApiErrorTypeSM exceptionType, string devMessage,
           string displayMessage = "", Exception innerException = null)
            : base(exceptionType, devMessage, displayMessage, innerException)
        { }
    }
}
