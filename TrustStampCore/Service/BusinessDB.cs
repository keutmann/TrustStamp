using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;

namespace TrustStampCore.Service
{
    public abstract class BusinessService : IDisposable 
    {
        public TrustStampDatabase DB = null;
        protected bool LocalDB = false;

        public BusinessService(TrustStampDatabase db)
        {
            DB = db;
        }

        public void Dispose()
        {
            if (LocalDB && DB != null)
                DB.Dispose();
        }
    }
}
