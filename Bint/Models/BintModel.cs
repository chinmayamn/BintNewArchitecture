using Bint.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
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
        public LoginViewModel LoginViewModel { get; set; }
     
        public MobileLoginViewModel MobileLoginViewModel { get; set; }

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
        public DataTable ActivityLogTable { get; set; }
    }
    public class UserCount
    {
        public int AdminCount { get; set; }
        public int InvestorCount { get; set; }
        public int ClientCount { get; set; }
        public int LockedUsersCount { get; set; }
        public int PartnerCount { get; set; }

        public int PendingAccessCount { get; set; }
        public int PendingKycCount { get; set; }
        public IEnumerable<ApplicationUser> AdminList { get; set; }
        public IEnumerable<ApplicationUser> InvestorList { get; set; }
        public IEnumerable<ApplicationUser> ClientList { get; set; }
        public IEnumerable<ApplicationUser> PartnerList { get; set; }

        public IEnumerable<ApplicationUser> LockedUsersList { get; set; }
    }
    public class AdminDashboard
    {
        public UserCount UserCount { get; set; }

        public decimal TotalUsd { get; set; }
        public decimal TotalBgc { get; set; }
        public DataTable AdminRequestDashboard { get; set; }
        public Dictionary<string,string> UsdInvestment { get; set; }
        public Payback Payback { get; set; }
        public DataSet UsdInvestmentMonthWise { get; set; }

    }

    public class SiteSettingDashboard
    {
        public string SmsBalance { get; set; }
        public RegId RegId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }

    public class SuperAdminDashboard
    {
        public UserCount UserCount { get; set; }
    }
    public class InvestorDashboard
    {
        public UserCount UserCount { get; set; }
        public decimal TotalUsd { get; set; }
        public decimal TotalBgc { get; set; }

        public Payback Payback { get; set; }
    }
    public class PartnerDashboard
    {
      public Payback Payback { get; set; }
    }
    public class ClientDashboard
    {
        public Payback Payback { get; set; }
    }
    public class Payback
    {
        public DataTable UsdPayback { get; set; }
        public DataTable UsdPaybackUser { get; set; }
    }
    public class SavedStatsDashboard
    {
        public IQueryable<CaptureDeviceData> Os { get; set; }
        public IQueryable<CaptureDeviceData> DeviceName { get; set; }
        public IQueryable<CaptureDeviceData> Browser { get; set; }
        public IQueryable<CaptureDeviceData> WifiMobile { get; set; }
        public IQueryable<CaptureDeviceData> BrandName { get; set; }
        public IQueryable<CaptureDeviceData> URole { get; set; }
        public IQueryable<CaptureDeviceData> PublicIp { get; set; }
    }
    public class RestrictedAccessDashboard
    {
        public IEnumerable<RestrictedAccess> RestrictedAccess { get; set; }
    }
    public class UsdDashboard
    {
        public DataTable RequestUsd { get; set; }
        public DataTable TransferUsd { get; set; }
        public DataTable Stats { get; set; }
        public string QrCode { get; set; }
        public string Tether { get; set; }
        public DataTable WithdrawUsd { get; set; }
        public DataTable DepositUsd { get; set; }

    }
    public class Doc
    {
        [Key]
        public int Id { get; set; }
        public string Userid { get; set; }
        public string Filename { get; set; }
        public string DocPath { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }
    public class CustomerUserCreate
    {
        public IEnumerable<ApplicationUser> AppUser { get; set; }
        public RegisterViewModel Reg { get; set; }

        public List<IdentityRole> URole { get; set; }

    }

    public class SuperAdmin
    {
        public IList<ApplicationUser> LoggedInUser { get; set; }
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
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LogoutTime { get; set; }
        public string OsName { get; set; }
        public string OsVersion { get; set; }
        public string OsPlatform { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string Brand { get; set; }
        public string BrandName { get; set; }
        public string Ipv4 { get; set; }

        public string Ipv6 { get; set; }
        public string Useragent { get; set; }

        public string PublicIp { get; set; }
       
        public string URole { get; set; }

    }

    public class RestrictedAccess
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime ErrorTime { get; set; }
    
        public string OsName { get; set; }
        public string OsVersion { get; set; }
        public string OsPlatform { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string Brand { get; set; }
        public string BrandName { get; set; }
        public string Ipv4 { get; set; }

        public string Ipv6 { get; set; }
        public string Useragent { get; set; }

        public string PublicIp { get; set; }

        public string Urole { get; set; }
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
    public class TransferUsd
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
        public ApplicationUser UserProfile { get; set; }
        public ActivityLogDashboard ActivityLogDashboard { get; set; }
        public DataTable UserDocs { get; set; }
        public IEnumerable<ApplicationUser> UserList { get; set; }

    }
    public enum TransferUsdStatusEnum
    {
        Requested, Transferred, Rejected, Debit, Credit,Received,Deposit,Withdraw,Accepted
    }
    public class UserProfileDoc
    {
        public DataTable UserDocs { get; set; }
    }
    public enum KycDocsStatusEnum
    {
        Accepted, Pending, Rejected
    }
    //public enum UserStatusEnum
    //{
    //    Verified, Pending, VerificationFailed, Processing
    //}
    public enum ActivityLogEnum
    {
        Person,Usd,Bgc,TransferUsd,ReceiveUsd,RequestUsd,Plan,Reject,Debit,Credit,DeletePerson,Pending,VerificationFailed,Verified,DepositUsd,WithdrawUsd,ConfirmDepositUsd,ConfirmWithdrawUsd
    }
    
    public class DepositWithdraw
    { 
        [Key]
        public int Id { get; set; }
        public string UsdAction { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
        public string TransactionId { get; set; }
        public Decimal Amount { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }

}
