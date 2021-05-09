using System;
using Bint.Constants;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace BintTest.Constants
{
    [ExcludeFromCodeCoverage]
    public class DbConstantTests
    {
        private readonly IDbConstants _dbConstants;
        public DbConstantTests()
        {
            _dbConstants = new DbConstants();
        }

        [Fact]
        public void SpGetrequestusdreport_Successfully()
        {
            Assert.Equal("sp_getrequestusdreport", _dbConstants.SpGetrequestusdreport);
        }
        [Fact]
        public void SpGettransferusdreport_Successfully()
        {
            Assert.Equal("sp_gettransferusdreport", _dbConstants.SpGettransferusdreport);
        }
        [Fact]
        public void SpGetuseractivity_Successfully()
        {
            Assert.Equal("sp_getuseractivity", _dbConstants.SpGetuseractivity);
        }
        [Fact]
        public void SpGetalertstats_Successfully()
        {
            Assert.Equal("sp_getalertstats", _dbConstants.SpGetalertstats);
        }
        [Fact]
        public void SpGetkycdocs_Successfully()
        {
            Assert.Equal("sp_getkycdocs", _dbConstants.SpGetkycdocs);
        }
        [Fact]
        public void SpGetdepositwithdrawusdrequests_Successfully()
        {
            Assert.Equal("sp_getdepositwithdrawusdrequests", _dbConstants.SpGetdepositwithdrawusdrequests);
        }
        [Fact]
        public void SpGetdepositwithdrawusdrequestsadmin_Successfully()
        {
            Assert.Equal("sp_getdepositwithdrawusdrequestsadmin", _dbConstants.SpGetdepositwithdrawusdrequestsadmin);
        }
        [Fact]
        public void SpAdminrequestdashboard_Successfully()
        {
            Assert.Equal("sp_adminrequestdashboard", _dbConstants.SpAdminrequestdashboard);
        }
        [Fact]
        public void SpGetusdinvestment_Successfully()
        {
            Assert.Equal("sp_getusdinvestment", _dbConstants.SpGetusdinvestment);
        }
        [Fact]
        public void SpGetusdpayback_Successfully()
        {
            Assert.Equal("sp_getusdpayback", _dbConstants.SpGetusdpayback);
        }
        [Fact]
        public void SpGetusdpaybackuser_Successfully()
        {
            Assert.Equal("sp_getusdpaybackuser", _dbConstants.SpGetusdpaybackuser);
        }
        [Fact]
        public void SpGetusdinvestmentmonthwise_Successfully()
        {
            Assert.Equal("sp_getusdinvestmentmonthwise", _dbConstants.SpGetusdinvestmentmonthwise);
        }
        [Fact]
        public void IndianZone_Successfully()
        {
            //Assert.Equal("(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi", _dbConstants.IndianZone.ToString());
            DateTime dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _dbConstants.IndianZone);
        }
    }
}
