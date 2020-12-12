using System.Collections.Generic;
using Framework.Core;
using Framework.Core.DataFilter;
using Sales.Query.Model.Model.QuotaTransactions;
using Sales.Query.Model.Model.QuotaTransactions.QuotaTransactionItems;
using Sales.Query.Model.Model.QuotaTransactions.QuotaTransactionsType;
using Sales.Query.Model.Model.SaleAgentQuotasQuery;

namespace Sales.Gateway.Facade.Contracts.QuotaTransactions.QueryServices
{
    public interface IQuotaTransactionQueryFacade : IFacadeService
    {
        List<QuotaTransactionItemQuery> GetAllQuotaTransactionItemQueryBy(long quotaTransactionId);
        PageResult<QuotaTransactionQuery> GetAllBy(QuotaTransactionType quotaTransactionType, PageInfo pageInfo);
        PageResult<SaleAgentQuotaQuery> GetAllSaleAgentQuotaQueryBy(long id, string keyword, PageInfo pageInfo, SortingInfo sortingInfo);

    }
}