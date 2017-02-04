using NUnit.Framework;
using System;
using System.Diagnostics;
using TrustStampCore.Repository;
using TrustStampCore.Service;

namespace TrustStampTests
{
    [SetUpFixture]
    public class SetupFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Use in memory database
            //TimeStampDatabase.IsMemoryDatabase = true; No need, see App.Config["test"] = true;
            App.Config["test"] = true; // Run as test, real timestamp not created!
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            //  all tests in the assembly have been run

        }
    }
}