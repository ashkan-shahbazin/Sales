using System.Collections.Generic;
using Framework.Domain;
using Sales.Domain.Model.Quotas;

namespace Sales.Domain.Model.QuotaTransactions
{
    public interface IQuotaTransactionRepository: IRepository
    {
        QuotaTransactionsId GetNextId();
        T Get<T>(QuotaTransactionsId id) where T : QuotaTransaction;
        void Create<T>(T aggregate) where T : QuotaTransaction;
        List<QuotaTransaction> GetAllAvailableBy(QuotaId quotaId);
    }
}