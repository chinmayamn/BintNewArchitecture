using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;
namespace Bint.Models
{
    public class DBFunc
    {
        static SqlConnection conn;
        public readonly ILogger _logger;
       
        public DBFunc(ILogger logger)
        {
            _logger = logger;
            var configuation = GetConfiguration();
            conn = new SqlConnection(configuation.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
           
        }

        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
        public DataTable GetRequestUSDReport(string userid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(conn.ConnectionString);
                SqlCommand cmd = new SqlCommand("sp_getrequestusdreport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
           
        }
        public DataTable GetTransferUSDReport(string userid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(conn.ConnectionString);
                SqlCommand cmd = new SqlCommand("sp_gettransferusdreport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
           
        }
        public DataTable GetUserActivityLog(string userid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(conn.ConnectionString);
                SqlCommand cmd = new SqlCommand("sp_getuseractivity", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
           
        }

        public DataTable GetAlertStats(string userid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(conn.ConnectionString);
                SqlCommand cmd = new SqlCommand("sp_getalertstats", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
           
        }

        public DataTable GetKYCDocs(string userid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(conn.ConnectionString);
                SqlCommand cmd = new SqlCommand("sp_getkycdocs", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;

            }
          
        }
        public DataTable GetDepositWithdrawUSDRequests(string userid,string action)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(conn.ConnectionString);
                SqlCommand cmd = new SqlCommand("sp_getdepositwithdrawusdrequests", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@action", action);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
           
        }
        public DataTable GetDepositWithdrawUSDRequestsadmin(string action)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(conn.ConnectionString);
                SqlCommand cmd = new SqlCommand("sp_getdepositwithdrawusdrequestsadmin", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", action);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
            return dt;
        }
        public DataTable GetAdminRequestDashboard()
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(conn.ConnectionString);
                SqlCommand cmd = new SqlCommand("sp_adminrequestdashboard", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
          
        }
        public DataTable GetUSDInvestment()
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(conn.ConnectionString);
                SqlCommand cmd = new SqlCommand("sp_getusdinvestment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
          
        }
        public DataTable GetUSDPayback(string userid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(conn.ConnectionString);
                SqlCommand cmd = new SqlCommand("sp_getusdpayback", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
            
        }
        public DataTable GetUSDPaybackUser(string userid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(conn.ConnectionString);
                SqlCommand cmd = new SqlCommand("sp_getusdpaybackuser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
            
        }
        public DataSet GetUSDInvestmentMonthwise()
        {
            DataSet dt = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(conn.ConnectionString);
                SqlCommand cmd = new SqlCommand("sp_getusdinvestmentmonthwise", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }

        }
    }
}
