using Microsoft.AspNetCore.Mvc;
using Sales.Gateway.Facade.Contracts.QuotaTransactions.CommandServices.Charges;

namespace Sales.Gateways.RestApi.Controllers.QuotaTransactions.Charges
{
    [Route("Sales/api/[controller]")]
    [ApiController]
    public class ChargeTransactionsController : Controller
    {
        private readonly IChargeTransactionFacade _chargeTransactionFacade;

        public ChargeTransactionsController(IChargeTransactionFacade chargeTransactionFacade)
        {
            _chargeTransactionFacade = chargeTransactionFacade;
        }

        [HttpPost("quota/excel-file")]
        public long Post()
        {
            return _chargeTransactionFacade.ChargeByExcelFile(Request.Form.Files[0]); 
        }
    }
}