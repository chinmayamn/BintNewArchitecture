using Bint.Constants;
using Bint.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Bint.Services
{
    public class DbFunc : IDbFunc
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IDbConstants _dbConstants;
        private static SqlConnection _conn;
        private readonly ApplicationDbContext _context;

        public DbFunc(ILogger logger, IConfiguration configuration, IDbConstants dbConstants)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConstants = dbConstants;
            var configurations = GetConfiguration();
            _conn = new SqlConnection(configurations.GetSection("ConnectionStrings").GetSection("DefaultConnection")
                .Value);
        }

        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);
            return builder.Build();
        }

        public DataTable GetRequestUsdReport(string userid)
        {
            var dt = new DataTable();
            try
            {
                var con = new SqlConnection(_conn.ConnectionString);
                var cmd = new SqlCommand(_dbConstants.SpGetrequestusdreport, con)
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
                var con = new SqlConnection(_conn.ConnectionString);
                var cmd = new SqlCommand(_dbConstants.SpGettransferusdreport, con)
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
                var con = new SqlConnection(_conn.ConnectionString);
                var cmd = new SqlCommand(_dbConstants.SpGetuseractivity, con)
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
                var con = new SqlConnection(_conn.ConnectionString);
                var cmd = new SqlCommand(_dbConstants.SpGetalertstats, con)
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
                var con = new SqlConnection(_conn.ConnectionString);
                var cmd = new SqlCommand(_dbConstants.SpGetkycdocs, con)
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
                var con = new SqlConnection(_conn.ConnectionString);
                var cmd = new SqlCommand(_dbConstants.SpGetdepositwithdrawusdrequests, con)
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
                var con = new SqlConnection(_conn.ConnectionString);
                var cmd = new SqlCommand(_dbConstants.SpGetdepositwithdrawusdrequestsadmin, con)
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
                var con = new SqlConnection(_conn.ConnectionString);
                var cmd = new SqlCommand(_dbConstants.SpAdminrequestdashboard, con)
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
                var con = new SqlConnection(_conn.ConnectionString);
                var cmd = new SqlCommand(_dbConstants.SpGetusdinvestment, con)
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
                var con = new SqlConnection(_conn.ConnectionString);
                var cmd = new SqlCommand(_dbConstants.SpGetusdpayback, con)
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
                var con = new SqlConnection(_conn.ConnectionString);
                var cmd = new SqlCommand(_dbConstants.SpGetusdpaybackuser, con)
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
                var con = new SqlConnection(_conn.ConnectionString);
                var cmd = new SqlCommand(_dbConstants.SpGetusdinvestmentmonthwise, con)
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