using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Bint.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

        private string firstname;
        private string lastname;
       
        private string mobile;
        private string address;
        private string bankname;
        private string bankaccount;
        private string ifsc_code;
        private string account_holder_name;

        private DateTime created_on;
        private string created_by;
        private string created_id;
        
        private string pan;
        private string upi_id;
        private string status;
        private string profilepicture;
        private string otp;
        private string userid;
        private decimal usd;
        private decimal bgc;
        private string kyc;
        private string tetheraddress { get; set; }
        private string QR { get; set; }
        public string UserId { get { return userid; } set { userid = value; } }
        public string Firstname { get { return firstname; } set { firstname = value; } }
        public string Lastname { get { return lastname; } set { lastname = value; } }
      
        public string Mobile { get { return mobile; } set { mobile = value; } }
        public string Address { get { return address; } set { address = value; } }
        public string Bankname { get { return bankname; } set { bankname = value; } }
        public string Bankaccount { get { return bankaccount; } set { bankaccount = value; } }
        public string Ifsc_code { get { return ifsc_code; } set { ifsc_code = value; } }
        public string Account_holder_name { get { return account_holder_name; } set { account_holder_name = value; } }

        public DateTime Created_on { get { return created_on; } set { created_on = value; } }
        public string Created_by { get { return created_by; } set { created_by = value; } }
        public string Created_id { get { return created_id; } set { created_id = value; } }
        public string Pan { get { return pan; } set { pan = value; } }
        public string Upi_id { get { return upi_id; } set { upi_id = value; } }
        public string Status { get { return status; } set { status = value; } }
        public string Profilepicture { get { return profilepicture; } set { profilepicture = value; } }

        public string OTP { get { return otp; } set { otp = value; } }
        public string KYC { get { return kyc; } set { kyc = value; } }
        public Decimal USD { get { return usd; } set { usd = value; } }
        public Decimal BGC { get { return bgc; } set { bgc = value; } }
        public string TetherAddress { get; set; }
        public string QRCode { get; set; }
        public string AdminTetherAddress { get; set; }
        public string AdminQRCode { get; set; }
        public string InvestorTetherAddress { get; set; }
        public string InvestorQRCode { get; set; }
        public string PartnerTetherAddress { get; set; }
        public string PartnerQRCode { get; set; }
        public string ClientTetherAddress { get; set; }
        public string ClientQRCode { get; set; }
    }
}
