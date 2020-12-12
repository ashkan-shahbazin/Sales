using Framework.Core;
using Microsoft.AspNetCore.Http;

namespace Sales.Gateway.Facade.Contracts.QuotaTransactions.CommandServices.Charges
{
    public interface IChargeTransactionFacade : IFacadeService
    {
        long ChargeByExcelFile(IFormFile excelFile);
    }
}