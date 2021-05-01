using System;
using Microsoft.AspNetCore.Identity;

namespace Bint.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string IfscCode { get; set; }
        public string AccountHolderName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedId { get; set; }
        public string Pan { get; set; }
        public string UpiId { get; set; }
        public string Status { get; set; }
        public string ProfilePicture { get; set; }
        public string Otp { get; set; }
        public string Kyc { get; set; }
        public decimal Usd { get; set; }
        public decimal Bgc { get; set; }
        public string TetherAddress { get; set; }
        public string QrCode { get; set; }
        public string AdminTetherAddress { get; set; }
        public string AdminQrCode { get; set; }
        public string InvestorTetherAddress { get; set; }
        public string InvestorQrCode { get; set; }
        public string PartnerTetherAddress { get; set; }
        public string PartnerQrCode { get; set; }
        public string ClientTetherAddress { get; set; }
        public string ClientQrCode { get; set; }
    }
}