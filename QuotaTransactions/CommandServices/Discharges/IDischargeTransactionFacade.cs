using Framework.Core;
using Microsoft.AspNetCore.Http;

namespace Sales.Gateway.Facade.Contracts.QuotaTransactions.CommandServices.Discharges
{
    public interface IDischargeTransactionFacade : IFacadeService
    {
        long DischargeByExcelFile(IFormFile excelFile);
    }
}