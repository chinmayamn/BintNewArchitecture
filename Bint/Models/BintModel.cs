using Bint.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
namespace Bint.Models
{
    
    public class RoleEdit
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<ApplicationUser> Members { get; set; }
        public IEnumerable<ApplicationUser> NonMembers { get; set; }
    }
    public class RoleModification
    {
        [Required]
        public string RoleName { get; set; }

        public string RoleId { get; set; }

        public string[] AddIds { get; set; }

        public string[] DeleteIds { get; set; }
    }

    public class UserLogin //no fine tuning required
    {
        public LoginViewModel loginviewmodel { get; set; }
     
        public MobileLoginViewModel mobileloginviewmodel { get; set; }

    }
   
    public class ActivityLog
    {
        [Key]
        public int Id { get; set; }
        public string ActivityType { get; set; }
        public string Userid { get; set; }
        public DateTime ActivityDate { get; set; }
        public string Activity { get; set; }
       
    }
    public class ActivityLogDashboard
    {
        public DataTable activityLogTable { get; set; }
    }
    public class UserCount
    {
        public int AdminCount { get; set; }
        public int InvestorCount { get; set; }
        public int ClientCount { get; set; }
        public int LockedUsersCount { get; set; }
        public int PartnerCount { get; set; }

        public int PendingAccessCount { get; set; }
        public int PendingKYCCount { get; set; }
        public IEnumerable<ApplicationUser> AdminList { get; set; }
        public IEnumerable<ApplicationUser> InvestorList { get; set; }
        public IEnumerable<ApplicationUser> ClientList { get; set; }
        public IEnumerable<ApplicationUser> PartnerList { get; set; }

        public IEnumerable<ApplicationUser> LockedUsersList { get; set; }
    }
    public class AdminDashboard
    {
        public UserCount userCount { get; set; }

        public decimal TotalUSD { get; set; }
        public decimal TotalBGC { get; set; }
        public DataTable AdminRequestDashboard { get; set; }
        public Dictionary<string,string> _USDInvestment { get; set; }
        public Payback _payback { get; set; }
        public DataSet _USDInvestmentMonthwise { get; set; }

    }

    public class SiteSettingDashboard
    {
        public string SMSBalance { get; set; }
        public RegId _regId { get; set; }
        public ApplicationUser u { get; set; }

    }

    public class SuperAdminDashboard
    {
        public UserCount userCount { get; set; }
    }
    public class InvestorDashboard
    {
        public UserCount userCount { get; set; }
        public decimal TotalUSD { get; set; }
        public decimal TotalBGC { get; set; }

        public Payback _payback { get; set; }
    }
    public class PartnerDashboard
    {
      public Payback _payback { get; set; }
    }
    public class ClientDashboard
    {
        public Payback _payback { get; set; }
    }
    public class Payback
    {
        public DataTable USDPayback { get; set; }
        public DataTable USDPaybackUser { get; set; }
    }
    public class SavedStatsDashboard
    {
        public IQueryable<CaptureDeviceData> _OS { get; set; }
        public IQueryable<CaptureDeviceData> _DeviceName { get; set; }
        public IQueryable<CaptureDeviceData> _Browser { get; set; }
        public IQueryable<CaptureDeviceData> _WifiMobile { get; set; }
        public IQueryable<CaptureDeviceData> _BrandName { get; set; }
        public IQueryable<CaptureDeviceData> _URole { get; set; }
        public IQueryable<CaptureDeviceData> _PublicIp { get; set; }
    }
    public class RestrictedAccessDashboard
    {
        public IEnumerable<RestrictedAccess> _restrictedaccess { get; set; }
    }
    public class USDDashboard
    {
        public DataTable _requestUSD { get; set; }
        public DataTable _transferUSD { get; set; }
        public DataTable _Stats { get; set; }
        public string _qrcode { get; set; }
        public string _tether { get; set; }
        public DataTable _withdrawUSD { get; set; }
        public DataTable _depositUSD { get; set; }

    }
    public class Doc
    {
        [Key]
        public int Id { get; set; }
        public string Userid { get; set; }
        public string Filename { get; set; }
        public string Docpath { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }
    public class CustomerUserCreate
    {
        public IEnumerable<ApplicationUser> appUser { get; set; }
        public RegisterViewModel reg { get; set; }

        public List<IdentityRole> urole { get; set; }

    }

    public class SuperAdmin
    {
        public IList<ApplicationUser> loggedInUser { get; set; }
    }

    public class ErrorLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Exception { get; set; }
        public string Properties { get; set; }
    }
    public class CaptureDeviceData
    {
        [Key]
        public int id { get; set; }
        public string userid { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LogoutTime { get; set; }
        public string OSName { get; set; }
        public string OSVersion { get; set; }
        public string OSPlatform { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string Brand { get; set; }
        public string BrandName { get; set; }
        public string IPv4 { get; set; }

        public string IPv6 { get; set; }
        public string Useragent { get; set; }

        public string PublicIp { get; set; }
       
        public string urole { get; set; }

    }

    public class RestrictedAccess
    {
        [Key]
        public int id { get; set; }
        public string userid { get; set; }
        public DateTime ErrorTime { get; set; }
    
        public string OSName { get; set; }
        public string OSVersion { get; set; }
        public string OSPlatform { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string Brand { get; set; }
        public string BrandName { get; set; }
        public string IPv4 { get; set; }

        public string IPv6 { get; set; }
        public string Useragent { get; set; }

        public string PublicIp { get; set; }

        public string urole { get; set; }
        public string ReturnUrl { get; set; }
        public string Verified { get; set; }

    }
   
    public class RegId
    {
        [Key]
        public int Id { get; set; }
        public string AdminId { get; set; }
        public string InvestorId { get; set; }
        public string PartnerId { get; set; }
        public string ClientId { get; set; }
    }
    public class TransferUSD
    {
        public int Id { get; set; }
        public string Userid { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public Decimal Amount { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? TransferDate { get; set; }
        public string FromStatus { get; set; }
        public string ToStatus { get; set; }
        public string Reason { get; set; }
        public decimal FromTotalAmount { get; set; }
        public decimal ToTotalAmount { get; set; }
    }
    public class AdminUserProfileDashboard
    {
        public ApplicationUser _userProfile { get; set; }
        public ActivityLogDashboard _activityLogDashboard { get; set; }
        public DataTable UserDocs { get; set; }
        public IEnumerable<ApplicationUser> UserList { get; set; }

    }
    public enum TransferUSDStatusEnum
    {
        Requested, Transferred, Rejected, Debit, Credit,Received,Deposit,Withdraw,Accepted
    }
    public class UserProfileDoc
    {
        public DataTable UserDocs { get; set; }
    }
    public enum KYCDocsStatusEnum
    {
        Accepted, Pending, Rejected
    }
    //public enum UserStatusEnum
    //{
    //    Verified, Pending, VerificationFailed, Processing
    //}
    public enum ActivityLogEnum
    {
        Person,USD,BGC,TransferUSD,ReceiveUSD,RequestUSD,Plan,Reject,Debit,Credit,DeletePerson,Pending,VerificationFailed,Verified,DepositUSD,WithdrawUSD,ConfirmDepositUSD,ConfirmWithdrawUSD
    }
    
    public class DepositWithdraw
    { 
        [Key]
        public int Id { get; set; }
        public string USDAction { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
        public string TransactionId { get; set; }
        public Decimal Amount { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }

}
