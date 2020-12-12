using Framework.Core.DataFilter;
using Framework.Domain;
using Sales.Query.Model.Model.QuotaTransactions.QuotaTransactionsType;

namespace Sales.Query.Model.Model.QuotaTransactions
{
    public interface IQuotaTransactionQueryRepository: IRepository
    {
        PageResult<QuotaTransactionQuery> GetAllBy(QuotaTransactionType quotaTransactionType, PageInfo pageInfo);
        QuotaTransactionQuery GetBy(long quotaTransactionId);
    }
}