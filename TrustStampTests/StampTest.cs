using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;

namespace TrustStampTests
{
    public abstract class StampTest
    {
        [SetUp]
        public virtual void Init()
        {
            using (var db = TimeStampDatabase.Open())
            {
                db.ProofTable.DropTable();
                db.BatchTable.DropTable();
                db.CreateIfNotExist();
            }
        }

        [TearDown]
        public virtual void Dispose()
        { /* ... */ }
    }
}
