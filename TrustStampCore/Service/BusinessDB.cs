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
        public TimeStampDatabase DB = null;
        protected bool LocalDB = false;

        //public static T Get<T>() where T : BusinessService, new()
        //{
        //    T obj = new T();
        //    obj.DB = TimeStampDatabase.Open();
        //    obj.LocalDB = true;
        //    return obj;
        //}

        public BusinessService(TimeStampDatabase db)
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
