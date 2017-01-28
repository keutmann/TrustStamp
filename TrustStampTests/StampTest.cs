using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampTests
{
    public abstract class StampTest
    {
        [SetUp]
        public virtual void Init()
        { /* ... */ }

        [TearDown]
        public virtual void Dispose()
        { /* ... */ }
    }
}
