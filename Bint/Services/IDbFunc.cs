using Microsoft.Extensions.Configuration;
using System.Data;

namespace Bint.Services
{
    public interface IDbFunc
    {
        IConfigurationRoot GetConfiguration();
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
