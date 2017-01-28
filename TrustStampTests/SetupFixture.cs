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
            // TODO: Add code here that is run before
            //  all tests in the assembly are run            
            TimeStampDatabase.DatabaseFilename = "Data Source=:memory:;Version=3;";
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // TODO: Add code here that is run after
            //  all tests in the assembly have been run
            //Console.WriteLine("Onetime teardown");
        }
    }
}