using Microsoft.EntityFrameworkCore;

namespace SuccessArenaDAL.Base
{
    public abstract class EfCoreContextRoot : DbContext, IEfCoreContextRoot
    {
        public EfCoreContextRoot(DbContextOptions options)
            : base(options)
        {
        }
    }
}
