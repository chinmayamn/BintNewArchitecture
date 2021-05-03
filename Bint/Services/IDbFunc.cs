using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Bint.Services
{
    public interface IDbFunc
    {
        DataTable GetRequestUsdReport(string userid);
        DataTable GetTransferUsdReport(string userid);
        DataTable GetUserActivityLog(string userid);
        DataTable GetAlertStats(string userid);
        DataTable GetKycDocs(string userid);
        DataTable GetDepositWithdrawUsdRequests(string userid, string action);
        DataTable GetDepositWithdrawUsdRequestsadmin(string action);
        DataTable GetAdminRequestDashboard();
        DataTable GetUsdInvestment();
        DataTable GetUsdPayback(string userid);
        DataTable GetUsdPaybackUser(string userid);
        DataSet GetUsdInvestmentMonthwise();
    }
}
