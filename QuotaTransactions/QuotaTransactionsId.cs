using Framework.Domain;

namespace Sales.Domain.Model.QuotaTransactions
{
    public class QuotaTransactionsId:IdBase<long>
    {

        public QuotaTransactionsId(long id)
            :base(id)
        {
        }
        protected QuotaTransactionsId() { }
    }
}