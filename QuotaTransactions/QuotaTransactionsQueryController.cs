using System.Collections.Generic;
using Framework.Core.DataFilter;
using Microsoft.AspNetCore.Mvc;
using Sales.Gateway.Facade.Contracts.QuotaTransactions.QueryServices;
using Sales.Query.Model.Model.QuotaTransactions;
using Sales.Query.Model.Model.QuotaTransactions.QuotaTransactionItems;
using Sales.Query.Model.Model.QuotaTransactions.QuotaTransactionsType;
using Sales.Query.Model.Model.SaleAgentQuotasQuery;

namespace Sales.Gateways.RestApi.Controllers.QuotaTransactions
{
    [Route("Sales/api/[controller]")]
    [ApiController]
    public class QuotaTransactionsQueryController : Controller
    {
        private readonly IQuotaTransactionQueryFacade _quotaTransactionQueryFacade;

        public QuotaTransactionsQueryController(IQuotaTransactionQueryFacade quotaTransactionQueryFacade)
        {
            _quotaTransactionQueryFacade = quotaTransactionQueryFacade;
        }

        [HttpGet("all-QuotaTransaction")]
        public PageResult<QuotaTransactionQuery> GetAllBy(QuotaTransactionType quotaTransactionType, [FromQuery] PageInfo pageInfo)
        {
            return _quotaTransactionQueryFacade.GetAllBy(quotaTransactionType, pageInfo);
        }
        [HttpGet("{id}/items")]
        public List<QuotaTransactionItemQuery> Get(long id)
        {
            return _quotaTransactionQueryFacade.GetAllQuotaTransactionItemQueryBy(id);
        }

        [HttpGet("saleAgentQuotas/{id}")]
        public PageResult<SaleAgentQuotaQuery> GetAllSaleAgentQuotaQueryBy(long id,string keyword, [FromQuery] PageInfo pageInfo,[FromQuery] SortingInfo sortingInfo)
        {
            return _quotaTransactionQueryFacade.GetAllSaleAgentQuotaQueryBy(id,keyword, pageInfo,sortingInfo);
        }
    }
}