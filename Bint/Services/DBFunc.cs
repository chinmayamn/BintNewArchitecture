using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Bint.Services
{
    public class DbFunc:IDbFunc
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public DbFunc(ILogger logger,IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public DataTable GetRequestUsdReport(string userid)
        {
            var dt = new DataTable();
            try
            {
                var con = new SqlConnection(_configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
                var cmd = new SqlCommand("sp_getrequestusdreport", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@userid", userid);
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
        }

        public DataTable GetTransferUsdReport(string userid)
        {
            var dt = new DataTable();
            try
            {
                var con = new SqlConnection(_configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
                var cmd = new SqlCommand("sp_gettransferusdreport", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@userid", userid);
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
        }

        public DataTable GetUserActivityLog(string userid)
        {
            var dt = new DataTable();
            try
            {
                var con = new SqlConnection(_configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
                var cmd = new SqlCommand("sp_getuseractivity", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@userid", userid);
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
        }

        public DataTable GetAlertStats(string userid)
        {
            var dt = new DataTable();
            try
            {
                var con = new SqlConnection(_configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
                var cmd = new SqlCommand("sp_getalertstats", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@userid", userid);
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
        }

        public DataTable GetKycDocs(string userid)
        {
            var dt = new DataTable();
            try
            {
                var con = new SqlConnection(_configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
                var cmd = new SqlCommand("sp_getkycdocs", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@userid", userid);
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
        }

        public DataTable GetDepositWithdrawUsdRequests(string userid, string action)
        {
            var dt = new DataTable();
            try
            {
                var con = new SqlConnection(_configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
                var cmd = new SqlCommand("sp_getdepositwithdrawusdrequests", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@action", action);
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
        }

        public DataTable GetDepositWithdrawUsdRequestsadmin(string action)
        {
            var dt = new DataTable();
            try
            {
                var con = new SqlConnection(_configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
                var cmd = new SqlCommand("sp_getdepositwithdrawusdrequestsadmin", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@action", action);
                var da = new SqlDataAdapter(cmd);
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
            var dt = new DataTable();
            try
            {
                var con = new SqlConnection(_configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
                var cmd = new SqlCommand("sp_adminrequestdashboard", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
        }

        public DataTable GetUsdInvestment()
        {
            var dt = new DataTable();
            try
            {
                var con = new SqlConnection(_configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
                var cmd = new SqlCommand("sp_getusdinvestment", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
        }

        public DataTable GetUsdPayback(string userid)
        {
            var dt = new DataTable();
            try
            {
                var con = new SqlConnection(_configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
                var cmd = new SqlCommand("sp_getusdpayback", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@userid", userid);
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
        }

        public DataTable GetUsdPaybackUser(string userid)
        {
            var dt = new DataTable();
            try
            {
                var con = new SqlConnection(_configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
                var cmd = new SqlCommand("sp_getusdpaybackuser", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@userid", userid);
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return dt;
            }
        }

        public DataSet GetUsdInvestmentMonthwise()
        {
            var dt = new DataSet();
            try
            {
                var con = new SqlConnection(_configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
                var cmd = new SqlCommand("sp_getusdinvestmentmonthwise", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var da = new SqlDataAdapter(cmd);
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