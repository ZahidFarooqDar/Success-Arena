using SuccessArenaFoundation.Foundation.AuthenticationHelper;
using SuccessArenaServiceModels.Foundation.Base.Interfaces;

namespace SuccessArenaFoundation.Foundation.Web.Security
{
    public class PasswordEncryptHelper : Rfc2898Helper, IPasswordEncryptHelper, IEncryptHelper
    {
        public PasswordEncryptHelper(string encryptionKey, string decryptionKey)
            : base(encryptionKey, decryptionKey)
        {
        }
    }
}
