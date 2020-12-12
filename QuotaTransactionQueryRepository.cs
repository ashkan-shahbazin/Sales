using System.Linq;
using Framework.Core.DataFilter;
using Sales.Query.Model.Model.QuotaTransactions;
using Sales.Query.Model.Model.QuotaTransactions.Charges;
using Sales.Query.Model.Model.QuotaTransactions.Discharges;
using Sales.Query.Model.Model.QuotaTransactions.QuotaTransactionsType;

namespace Sales.Query.EF.Repositories
{
    public class QuotaTransactionQueryRepository : IQuotaTransactionQueryRepository
    {
        private readonly SalesDenormalQueryContext _salesDenormalQueryContext;

        public QuotaTransactionQueryRepository(SalesDenormalQueryContext salesDenormalQueryContext)
        {
            _salesDenormalQueryContext = salesDenormalQueryContext;
        }

        public PageResult<QuotaTransactionQuery> GetAllBy(QuotaTransactionType quotaTransactionType, PageInfo pageInfo)
        { 
            if (!pageInfo.Take.HasValue)
                pageInfo.Take = 10;

            var query = _salesDenormalQueryContext.QuotaTransactionQueries.AsQueryable();

            if (quotaTransactionType == QuotaTransactionType.Charge)
                query = query.OfType<ChargeQuery>();

            else if (quotaTransactionType == QuotaTransactionType.Discharge)
                query = query.OfType<DischargeQuery>();

            long totalCount = query.Count();
            var reservationQueries = query.OrderByDescending(x => x.Id).Skip(pageInfo.Skip)
                .Take(pageInfo.Take.Value).ToList();
            return new PageResult<QuotaTransactionQuery>(reservationQueries, totalCount);
        }
        public QuotaTransactionQuery GetBy(long quotaTransactionId)
        {
            return _salesDenormalQueryContext.QuotaTransactionQueries.FirstOrDefault(x => x.Id == quotaTransactionId);
        }
    }
}