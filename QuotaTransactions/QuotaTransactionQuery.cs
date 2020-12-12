using System;
using System.Collections.Generic;
using Sales.Query.Model.Contracts.QuotaTransactions;
using Sales.Query.Model.Model.QuotaTransactions.QuotaTransactionItems;

namespace Sales.Query.Model.Model.QuotaTransactions
{
    public abstract class QuotaTransactionQuery
    {
        public long Id { get; set; }
        public DateTime CreateOn { get;  set; }
        public string PersianDateTime { get; set; }
        public SaleAgentInfoQuery SaleAgentInfoQuery { get;  set; } 
        public long QuotaId { get; private set; }
        public Reason Reason { get; private set; }
        public List<QuotaTransactionItemQuery> QuotaTransactionItemsQuery { get; set; }
        public long CreateUserId { get; set; }

        protected QuotaTransactionQuery(long id, DateTime createOn, string persianDateTime, SaleAgentInfoQuery saleAgentInfoQuery, long quotaId, Reason reason, List<QuotaTransactionItemQuery> quotaTransactionItemsQuery, long createUserId)
        {
            Id = id;
            CreateOn = createOn;
            PersianDateTime = persianDateTime;
            SaleAgentInfoQuery = saleAgentInfoQuery;
            QuotaId = quotaId;
            Reason = reason;
            QuotaTransactionItemsQuery = quotaTransactionItemsQuery;
            CreateUserId = createUserId;
        }

        protected QuotaTransactionQuery() { }
    }
}