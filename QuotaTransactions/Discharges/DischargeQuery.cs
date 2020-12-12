using System;
using System.Collections.Generic;
using Sales.Query.Model.Contracts.QuotaTransactions;
using Sales.Query.Model.Model.QuotaTransactions.QuotaTransactionItems;

namespace Sales.Query.Model.Model.QuotaTransactions.Discharges
{
    public class DischargeQuery : QuotaTransactionQuery
    {
        public DischargeQuery(long id, DateTime createOn, string persianDateTime, SaleAgentInfoQuery saleAgentInfoQuery
            , long quotaId, Reason reason, List<QuotaTransactionItemQuery> quotaTransactionItemsQuery, long createUserId)
            : base(id, createOn, persianDateTime, saleAgentInfoQuery, quotaId, reason, quotaTransactionItemsQuery, createUserId)
        {
        }

        protected DischargeQuery() { }
    }
}