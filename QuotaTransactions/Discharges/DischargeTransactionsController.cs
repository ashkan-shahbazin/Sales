using Microsoft.AspNetCore.Mvc;
using Sales.Gateway.Facade.Contracts.QuotaTransactions.CommandServices.Discharges;

namespace Sales.Gateways.RestApi.Controllers.QuotaTransactions.Discharges
{
    [Route("Sales/api/[controller]")]
    [ApiController]
    public class DischargeTransactionsController : Controller
    {
        private readonly IDischargeTransactionFacade _dischargeTransactionFacade;

        public DischargeTransactionsController(IDischargeTransactionFacade dischargeTransactionFacade)
        {
            _dischargeTransactionFacade = dischargeTransactionFacade;
        }

        [HttpPost("quota/excel-file")]
        public long Post()
        {
            return _dischargeTransactionFacade.DischargeByExcelFile(Request.Form.Files[0]);
        }
    }
}