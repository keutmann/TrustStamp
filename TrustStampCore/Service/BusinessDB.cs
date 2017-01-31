using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;

namespace TrustStampCore.Service
{
    public abstract class BusinessDB : IDisposable
    {
        public TimeStampDatabase DB = null;
        protected bool LocalDB = false;

        public BusinessDB(TimeStampDatabase db)
        {
            LocalDB = (db == null);
            if (LocalDB)
                DB = TimeStampDatabase.Open();
            else
                DB = db;
        }

        public void Dispose()
        {
            if (LocalDB && DB != null)
                DB.Dispose();
        }
    }
}
