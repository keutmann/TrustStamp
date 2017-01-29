using NUnit.Framework;
using System;
using System.Diagnostics;
using TrustStampCore.Repository;

namespace TrustStampTests
{
    [SetUpFixture]
    public class SetupFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Use in memory database
            TimeStampDatabase.IsMemoryDatabase = true;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            //  all tests in the assembly have been run

        }
    }
}